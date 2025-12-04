using CefSharp;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using EmbeddedBrowser.BrowserHelper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using ThreadUtils;
using TwtDominatorCore.Requests;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDLibrary.GeneralLibrary;
using Cookie = CefSharp.Cookie;

namespace TwtDominatorCore.TDUtility
{
    public class XPFFHeaderGenerator
    {
        private readonly string baseKey;

        public XPFFHeaderGenerator(string baseKey)
        {
            this.baseKey = baseKey;
        }

        private byte[] DeriveXpffKey(string guestId)
        {
            string combined = baseKey + guestId;
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
            }
        }

        public string GenerateXpff(string plaintext, string guestId)
        {
            try
            {
                byte[] key = DeriveXpffKey(guestId);
                byte[] nonce = new byte[12]; // 96-bit nonce
                new SecureRandom().NextBytes(nonce);

                byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
                byte[] ciphertext = new byte[plaintextBytes.Length + 16]; // +16 for tag

                AeadParameters parameters = new AeadParameters(new KeyParameter(key), 128, nonce);
                GcmBlockCipher cipher = new GcmBlockCipher(new Org.BouncyCastle.Crypto.Engines.AesEngine());
                cipher.Init(true, parameters); // true = encryption

                int len = cipher.ProcessBytes(plaintextBytes, 0, plaintextBytes.Length, ciphertext, 0);
                cipher.DoFinal(ciphertext, len);

                // Combine nonce + ciphertext+tag
                byte[] result = new byte[nonce.Length + ciphertext.Length];
                Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
                Buffer.BlockCopy(ciphertext, 0, result, nonce.Length, ciphertext.Length);

                return BitConverter.ToString(result).Replace("-", "").ToLower();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string DecodeXpff(string hexString, string guestId)
        {
            byte[] key = DeriveXpffKey(guestId);
            byte[] raw = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
            {
                raw[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            byte[] nonce = new byte[12];
            byte[] ciphertext = new byte[raw.Length - 12];

            Buffer.BlockCopy(raw, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(raw, nonce.Length, ciphertext, 0, ciphertext.Length);

            byte[] plaintext = new byte[ciphertext.Length - 16]; // minus GCM tag

            AeadParameters parameters = new AeadParameters(new KeyParameter(key), 128, nonce);
            GcmBlockCipher cipher = new GcmBlockCipher(new Org.BouncyCastle.Crypto.Engines.AesEngine());
            cipher.Init(false, parameters); // false = decryption

            int len = cipher.ProcessBytes(ciphertext, 0, ciphertext.Length, plaintext, 0);
            cipher.DoFinal(plaintext, len);

            return Encoding.UTF8.GetString(plaintext);
        }
    }


    public enum SearchType
    {
        Search,
        Follow,
        Tweet,
        Retweet,
        Repost,
        Like,
        Comment,
        Unfollow,
        None,
        CustomTweet,
        CustomUser,
        Unlike,
        Follower,
        Following,
        Retweeters,
        BookMark,
        QuoteTweet,
        Login
    }
    //TODO : should make non static
    public static class TdUtility
    {
        private static readonly Random Random = new Random();
        private static Dictionary<SearchType, List<string>> TransactionIDs = new Dictionary<SearchType, List<string>>
        {
            {
                SearchType.Search, new List<string>()
                {
                    "hAOS4H6W87rdlzjQ3FzKK+xh1imvuTIWlWvkRDFTWUwDNDWLLjWP4RRVxHmw/J6eaqwYRodaOqUmi030S/RW2DasaXY4hw",
                    "9XLjkQ/ngsus5kmhrS27Wp0Qp1jeyENn5BqVNUAiKD1yRUT6X0T+kGUktQjBje/vG71pN/YsDA8bNh7dvciGqMNpOJTT9g",
                    "DIsaaPYeezJVH7BYVNRCo2TpXqEnMbqeHeNszLnb0cSLvL0Dpr0HaZzdTPE4dBYW4muQzg8AvyhvmiCTQHs0Fu7a2RbeDw",
                    "gQaX5XuT9r/Ykj3V2VnPLulk0yyqvDcTkG7hQTRWXEkGMTCOKzCK5BFQwXy1+ZubbwMdQ4I2BrYJmk8UyAda6r1xc+zVgg",
                    "PborWccvSgNkLoFpZeVzklXYb5AWAIuvLNJd/Yjq4PW6jYwyl4w2WK3sfcAJRScn06eh/z7oR95o9t+51uaElEp5LjZ7Pg",
                    "JaIzQd83Uht8Nplxff1rik3Ad4gOGJO3NMpF5ZDy+O2ilZQqj5QuQLX0ZdgRXT8/y5O55yapXxv9ihirdGjf+X2i81LPJg",
                    "PC2cJlcOK3hMWZt6fgbLxV2kptfenXXGa8UYMbPxnAnfBFPxd5l3jpXr86VlChXmbBDM+D9ncUTobXALCNGL8XzvSIElPw",
                    "wdBh26rz1oWxpGaHg/s2OKBZWyojYIg7ljjlzE4MYfQi+a4MimSKc2gWDliY9+gbkacxBcLGkvOFX/Wc9AVCyyckv0bOwg"
                }
            },
            {
                SearchType.Like,new List<string>
                {
                    "zEvaqDbeu/KV33CYlBSCY6QpnmHn8Xpe3SOsDHkbEQRLfH3DZn3HqVwdjDH4tNbWIgBtDs/a/ivHPpwWr8Sx2kO4bUD1zw",
                    "5fRF/47X8qGVgEKjp98SHIR9fw4HRKwfshzB6GooRdAG3YoorkCuV0wyKny808w/tS0VIea8GU7BoQhF5BWPxojiQDXR5g",
                    "4/JD+YjR9KeThkSlodkUGoJ7eQgBQqoZtBrH7mwuQ9YA24wuqEaoUUo0LHq61co5syUTJ+BIblffXajb0CqGzB990gJ74A",
                    "FQS1D34nAlFlcLJTVy/i7HSNj/73tFzvQuwxGJrYtSD2LXrYXrBep7zC2oxMIzzPRdfl0RZGFXNUhZWInMaAP7UcNUGoFg",
                    "MiOSKFkAJXZCV5V0cAjFy1OqqNnQk3vIZcsWP73/kgfRCl3/eZd5gJvl/atrBBvoYvPC9jFtNYDhV7qaNETNUdqKP7LbMQ",
                    "SVjpUyJ7Xg05LO4PC3O+sCjR06Kr6ACzHrBtRMaE6XyqcSaEAuwC++CehtAQf2CTGfa5jUoECS7mAy0HKf6iIUI20GHESg",
                    "QlPiWClwVQYyJ+UEAHi1uyPa2Kmg4wu4FbtmT82P4nehei2PCecJ8OuVjdsbdGuYEviyhkG0e++lTW9QduIi/f6zJzzaQQ",
                    "obABu8qTtuXRxAbn45tWWMA5O0pDAOhb9liFrC5sAZRCmc5s6gTqEwh2bjj4l4h78RlRZaL9W5UALah6kVXC6N+y3NSwog",
                    "cmPSaBlAZTYCF9U0MEiFixPq6JmQ0zuIJYtWf/2/0keRSh2/Odc5wNulvesrRFuoIsSCtnGEktUvHWvps0HMxqjtg2llcQ"
                }
            },
            {
                SearchType.Comment,new List<string>
                {
                    "ef5vHYNrDkcgasUtIaE31hGcK9RSRM/raJYZucyupLH+ych208hyHOmoOYRNAWNjlzzbu3pvvZMTXjZmULqG+CMXp0oveg"
                }
            },
            {
                SearchType.CustomTweet,new List<string>
                {
                    "m+ID9qMCZhcpz6J3p3SMoklMO+Ta6k0KrBmEAytOdTt1J/zynJbV+GBOwbSkdd75/ftmX5ja/2L24ZETM2hVEKJm5PtPmA",
                    "18ObSkaPTiIQl0uIEbestnOxujyf0n3oKohX38sxHqJL8z1by80xWR0Y1vfiP7ZdLMqtFNRowsAXgHW6Fu3Gn54XUXoo1A",
                    "IVpAsJSP/Gj0ap5Gm+jEjmyvsV+uF8Dl8Y+UQxgX9B0o6EEJLPxIFHYfxgcElpm4HOLQ5SKeiL6e0gvIMlhJhiIGa7f+Ig",
                    "5Z6EdFBLOKwwrlqCXywASqhrdZtq0wQhNUtQh9zTMNnsLIXN6DiM0LLbAsPAUl182AIUIeYuxWO3xRCKdC+NO3jGtQCB5g",
                    "nOf9DSkyQdVJ1yP7JlV5M9ESDOITqn1YTDIp/qWqSaCVVfy0kUH1qcuie7q5KyQFoZ1uWJ8tlGhxYZNAtjfsFpb9qYZNnw",
                    "TjUv3/vgkwebBfEp9Ier4QPA3jDBeK+KnuD7LHd4m3JHhy5mQ5MnexlwqWhr+fbXc1q8ik3mdIiLMmWS/SA9RMtXe9vaTQ",
                    "mOP5CS02RdFN0yf/IlF9N9UWCOYXrnlcSDYt+qGuTaSRUfiwlUXxrc+mf769LyABpb5qXJtQ0DFmcXZPbDrLpgQzmGZ8mw",
                    "75SOflpBMqY6pFCIVSYKQKJhf5Fg2Q4rP0FajdbZOtPmJo/H4jKG2rjRCMnKWFd20q8dK+wK/ln3QrUVVcL0vU4OytR57A"
                }
            },
            {
                SearchType.CustomUser,new List<string>
                {
                    "Ss1cLrBYPXQTWfYeEpIE5SKvGOdhd/zYW6Uqiv+dl4LN+vtF4PtBL9qbCrd+MlBQpP7uiElo2+X/dze/tuEC/5PUW2j/SQ"
                }
            },
            {
                SearchType.Repost,new List<string>
                {
                    "2ZuZHohZZbLIT9bOV5+RkrKgSXfISYlEbTb0XAsvy4BrBxTFrDF8D4Fm3MsthkoLdgtzG9pW8wGmnLDrx47URa1mqvSO2g",
                    "/CL8Es8BiePmdO+1/jVrpA5gjfsnPdaNh6ff1C7XGyLl3jY2TOvRbCbnkHaccmXNjawKOP98qU13r+/Hj9JM5GrjMmil/w",
                    "6DboBtsVnffyYPuh6iF/sBp0me8zKcKZk7PLwDrDDzbxyiIiWP/FeDLzhGKIZnHZmaUeLOsnoqe6HCQCiLVTs4edr2jk6w",
                    "gV+Bb7J89J6bCZLIg0gW2XMd8IZaQKvw+tqiqVOqZl+Yo0tLMZasEVua7QvhDxiw8Mp3RYLMh4lHKj1ms02paKcvbZszgg",
                    "JvgmyBXbUzk8rjVvJO+xftS6VyH95wxXXX0FDvQNwfg/BOzsljELtvw9SqxGqL8XV27Q4iV9P+kCmnvrLEqsbaIO5r0fJQ",
                    "s22zXYBOxqypO6D6sXok60EvwrRocpnCyOiQm2GYVG2qkXl5A6SeI2mo3znTPSqCwvVFd7A7y83ymPSc1pnJG1zTFt/HsA",
                    "hliGaLV785mcDpXPhE8R3nQa94FdR6z3/d2lrlStYVifpExMNpGrFlyd6gzmCB+398VwQoV2sWhyvUeOl7PtImCPO07HhQ",
                    "lEqUeqdp4YuOHIfdll0DzGYI5ZNPVb7l78+3vEa/c0qNtl5eJIO5BE6P+B70Gg2l5dRiUJf2UFJHi6z6+B9dIgRbB6hhlw"
                }
            },
            {
                SearchType.Retweet,new List<string>
                {
                    "3pyeGY9eYrXPSNHJUJiWlbWnTnDPTo5DajHzWwwozIdsABPCqzZ7CIZh28wqgU0McS9yHN0QuOlU9E43D2aOGUpbD4cZ3Q"
                }
            },
            {
                SearchType.Unlike,new List<string>
                {
                    "oebgHIOG1CGUs04qWScbLz+NDpWOAoNcbdCtEwUjK2KySqu/6tvFLhzSJFloA2yJPSTQYqJ05Tfz4d89GeNh/ch4cZt8og",
                    "4jziDNEfl/34avGr4Ct1uhB+k+U5I8iTmbnByjDJBTz7wCgoUvXPcjj5jmiCbHvTk00XJuFOe+M8AFDkLqvO5X8DiB8o4Q",
                    "U41TvWCuJkxJ20AaUZrEC6HPIlSIknkiKAhwe4F4tI1KcZmZ40R+w4lIP9kz3cpiIv2ml1C++rHFql/mfJWpmGouw1DwUA",
                    "bLJsgl+RGXN25H8lbqX7NJ7wHWu3rUYdFzdPRL5Hi7J1Tqam3HtB/LZ3AOYM4vVdHcCZqG8VzevAngQoPXtV8yGn0xB8bw",
                    "rnCuQJ1T27G0Jr3nrGc59lwy36l1b4Tf1fWNhnyFSXC3jGRkHrmDPnS1wiTOIDef3wRbaq325MeJ5iL0RQ/gHE20WJIyrQ",
                    "k02TfaBu5oyJG4DakVoEy2EP4pRIUrni6Miwu0G4dE2KsVlZI4S+A0mI/xnzHQqi4jpmV5AcjuSoNt0fdGTjOsR04pZpkA",
                    "+Cb4FssFjeficOux+jFvoApkif8jOdKJg6Pb0CrTHybh2jIySO/VaCLjlHKYdmHJiV4NPPsVwVK/8mfY3PTBavaQXigM+w",
                    "Me8x3wLMRC4ruSJ4M/imacOtQDbq8BtASmoSGeMa1u8oE/v7gSYcoesqXbtRv6gAQJXE9TKQv5aHWf56iXxFvwANzAJnMg"
                }
            },
            {
                SearchType.Follower,new List<string>
                {
                    "quxNGvgANj0qwPqTm9KM6G/P362E6gZV0Eajihd4wvWZgQbY5JBH8ii6UDA/q/b+VstPR6kDA8CUetMeOWwrC0Wgkk8yqQ",
                    "3Z0DQI1M0VK88vrBTO71szyRjqgLgtaVDcW5FmRVWjNNAKIhEejvKlClefGfZTAMgOw1MN6/IPqh5nfcKTv07Y0ic7uC3g",
                    "2LqlqcoMPPUEVCvOefZVy/bxBtGJ0N7heMP7P6X6ZixRMPXBiVOgx7l9GEBSU9yRqO0wNdsoTERjNv/Z23B6FU47ZO1K2w",
                    "fWVvmeFWcZL2UXy/Hy5Rb5QSc9SHo1y7bItUH37VMdGc/Emb00OlajtIJ7NY7uiqgEWVkH52NtQA6mL6pQVvGgpBMbGLfg",
                    "OkW3HdX6uoDxSJienXOujdFeRFn+v0dqOKSV4rOMLmYMkr8/zbh83Enle9NBbOjjUAbS1znJ+qcxKA09Ctn3kC77twkxOQ",
                    "dccb1zdAEr53Zanp8SKCLSLZVji+ZwNiqgicEyAQYWGvdnkapmX+3fk40PGF8RCrEUqdmHaqFxgfcKTeAaoiXrGxdsCldg",
                    "jx2wsodgRaDX5lGNX8/Qg5V0aFWGuoiuoAu9U01poBgZBVFW4Xf6wXvR2ChQtBZcI0pnYozdoAl+58s1K1AwReJw2503jA",
                    "y096je7dPo2lxeLmj1J6qgTGmIZf5q00KzDtGESDdNcNqBIrMojq7kGOwQ7WmyPpVCUjJsizKyVNLt9z4t5CcAZBoAONyA"
                }
            },
            {
                SearchType.Retweeters,new List<string>
                {
                    "JwX+8oCTJ7sFIEZfrRFKJE/Kgql32Nl/cB9byU0AyauagboJDFlVKkcxlq6A/cEB7mmm5CTuGJaTdTQD3QVp4ZnoVH3SJA",
                    "7xnzaUsHXX3EVKDw8ooBhcX9l7GwePzpzj4bslj2Y7dHlD58l5cnMa12cBoUutUddY0cK+yLBqoXGp+RfiC3hJ+l4I7V7A",
                    "BCPJ6hd5HNREstpVEY4usHM49f4GRv+LnTho0nKv6dy7ku6U6f6CxGgIH3Zc33laNHj3wAf6msuKQ9b7JS6evI5puoQjBw",
                    "7nlYAonevLAgfHBxh7IWTlN5WJWSyt1aZT05D+IlLLSPBPUICw7y5QqDk1RTeSSjI3odKu2J4RsOeEpzQrWFwE8tNmLz7Q",
                    "26WWprxcC7dMUESCA/Ln2fbEU4EQarcqoGn4dlL8j3trxK1lJhEqIMAKwa1wdQjh/2ooH9jISUzei8xNpqUtRL+kfyWo2A",
                    "H9696pdEF+Tt5odDjf5AZziROJgAF3thN/yysovDGgKKAlp+nzy0KTznSCxS4CACSNHs2xxpn9jf2FSMBzUkO1OiYEL9HA",
                    "et1isIZIEsfohDc6qXJvWvoLl3fEskXaxf73V2YnbIiW48Pg/1yZXth6E+XBaaaj7pWJvnmWIFHWOcSpuPOMa+DJO6dBeQ",
                    "RClYuXy5o70pfhTNmRUWuGZmPnmHo/p30oIGlPpCW1g2xDh6f2xZfdKsCOl2bvTjzkawgEfyUpDuJRL2Kxg6FAfvo4kCRw"
                }
            },
            {
                SearchType.BookMark,new List<string>
                {
                    "YME36qPgi1BjMHKn4f0oN5vf6Y5UCuICOcu8iVGVraJfKw0OcihD7GHgIZiRk9SfzpySf2T5SKAOzBQBRacCxp5bm5u2Yw",
                    "w2KUSQBDKPPAk9EEQl6LlDh8Si33qUGhmmgfKvI2DgH8iK6t0YvgT8JDgjsyMHc8beU23Mclx4nOoVFJzssNDe0pJbkNwA",
                    "V/YA3ZTXvGdUB0WQ1sofAKzo3rljPdU1DvyLvmaimpVoHDo5RR9021bXFq+mpOOo+XCiSFO46MxPlikKxOngx1QyOn/aVA",
                    "9lehfDV2Hcb1puQxd2u+oQ1JfxjCnHSUr10qH8cDOzTJvZuY5L7Vevd2tw4HBUIJWN8D6fLcr75hHMxkGvkxgILzeSbi9Q",
                    "OJlvsvu40wg7aCr/uaVwb8OHsdYMUrpaYZPk0QnN9foHc1VWKnAbtDm4ecDJy4zHlhLNJzzZGLcju5ow8Fl5ZFZkLRyHOw",
                    "edgu87r5kkl6KWu++OQxLoLG8JdNE/sbINKlkEiMtLtGMhQXazFa9Xj5OIGIis2G11eMZn3f2drR9CsCIbg3XEmehcuzeg",
                    "ddQi/7b1nkV2JWey9Og9Io7K/JtBH/cXLN6pnESAuLdKPhgbZz1W+XT1NI2EhsGK20WAanEDSABdWpQKM1QJcW0CzelBdg",
                    "nTzKF14ddq2ezY9aHADVymYiFHOp9x//xDZBdKxoUF+i1vDzj9W+EZwd3GVsbiliM69ogplIIvRALFVr/IAzZf62dsbBng"
                }
            },
            {
                SearchType.QuoteTweet,new List<string>
                {
                    "9M5IdNy0DoEE1JuMVnDmNKmlCP3TAH6hfXEmZZcxLyrPugup6nY4zuoUJxGvFdborA/V1PD8Am+cBmVdsLMIsO8nzB/+9w",
                    "W2Hn23MboS6rezQj+d9JmwYKp1J8r9EO0t6JyjiegIVgFaQGRdmXYUW7iL4AunlHA2Z9e18QYaL/e+VMh62W/+JwvsGgWA",
                    "981Ld9+3DYIH15iPVXPlN6qmC/7QA32ifnIlZpQyLCnMuQiq6XU7zekXJBKsFtXrr6vR1/OVt6hvO1nOe2TnHke86I989A",
                    "dE7I9Fw0jgGEVBsM1vBmtCkliH1TgP4h/fGm5Rexr6pPOospava4TmqUp5EvlVZoLPVSVHDDgzNq9xDwoNcIE08s0XrQdw",
                    "ppwaJo7mXNNWhsneBCK0Zvv3Wq+BUizzLyN0N8VjfXid6Fn7uCRqnLhGdUP9R4S6/haAhqJaBwdiov2S6c8O29U1CMS8pQ",
                    "CzG3iyNL8X77K2RzqY8Zy1Za9wIs/4Fego7ZmmjO0NUwRfRWFYnHMRXr2O5Q6ikXU80tKw8ECMea+/2Ep8N6Hb8UdnTuCA",
                    "Ylje4koimBeSQg0awOZwoj8znmtFlug36+ew8wGnubxZLJ0/fOCuWHyCsYc5g0B+OodEQmY1b+KJyLJG2dqyt9o/I+owYQ",
                    "/8VDf9e/BYoP35CHXXvtP6KuA/bYC3Wqdnotbpw6JCHEsQCi4X0zxeEfLBqkHt3jpwLZ3/vfYZbOANo6Z6rf7j+g0kKf/A"
                }
            }
        };
        public static string GetTransactionID(SearchType type) => TransactionIDs.ContainsKey(type) ? TransactionIDs[type].GetRandomItem() : string.Empty;
        public static readonly object LockFile = new object();
        public static string GetXClientTransactionID(string Method,string URL)
        {
            var transactionId = string.Empty;
            var url = "https://www.dropbox.com/scl/fi/p21mpekyx92fvc6kopf08/Runner.exe?rlkey=oa91ohho2ch5dluzzwm21c21u&e=1&dl=1";
            //var url = "https://download1321.mediafire.com/htswgjonk2igqHAr9J34hC3gd9mAhyWgrk2GSa7kFVac9a5U1FMZxxhmOaj3Yj-Z5wHSIyiqeeSxY9rADyuO9RDq_LoC6HZ6_rIie1MDWaHZNdwEZlg7mu6o1eSrCbAtQCRJAWlwVU7mZA-Co9lNTiDMAWhCOTRyLRuYaIjUE5lee6o/g8dvmlvroya6hux/Runner.exe";
            var filePath = Path.Combine(ConstantVariable.GetOtherDir(), $"Runner_{Assembly.GetExecutingAssembly().GetName().Version}.exe");
            if (!File.Exists(filePath))
            {
                //Download the file from the URL
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        using (HttpResponseMessage response = client.GetAsync(url).Result)
                        {
                            response.EnsureSuccessStatusCode();

                            using (Stream contentStream = response.Content.ReadAsStreamAsync().Result,
                                          fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                contentStream.CopyTo(fileStream);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return transactionId;
                    }
                }
            }
            //Download And Store at location to get the transaction ID from exe.
            var arguments = $"--method={Method} --url={URL}";
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = filePath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    transactionId = Utilities.GetBetween(output, "X_Client_Transaction_ID:", "<$$>");
                }
            }
            catch (Exception)
            {
                return transactionId;
            }
            return transactionId;
        }
        public static string GetPostAuthenticityToken(string data, string paramName)
        {
            var value = string.Empty;
            var startIndx = data.IndexOf(paramName, StringComparison.Ordinal);
            if (startIndx > 0)
            {
                var endIndx = data.IndexOf("\"", startIndx, StringComparison.Ordinal);
                value = data.Substring(startIndx, endIndx - startIndx).Replace(",", "");
                if (value.Contains(paramName))
                    try
                    {
                        var getOuthentication = Regex.Split(data, "\"postAuthenticityToken\":\"");
                        var authenticity = Regex.Split(getOuthentication[1], ",");

                        if (authenticity[0].IndexOf("\"", StringComparison.Ordinal) > 0)
                        {
                            var indexStart1 = authenticity[0].IndexOf("\"", StringComparison.Ordinal);
                            var start = authenticity[0].Substring(0, indexStart1);
                            value = start.Replace("\"", "").Replace(":", "");
                        }
                    }
                    catch
                    {
                    }

                return value;
            }

            var array = Regex.Split(data, "<input type=\"hidden\"");
            foreach (var item in array)
                if (item.Contains("authenticity_token"))
                {
                    var startindex = item.IndexOf("value=\"", StringComparison.Ordinal);
                    if (startindex > 0)
                    {
                        var start = item.Substring(startindex).Replace("value=\"", "");
                        var endIndex = start.IndexOf("\"", StringComparison.Ordinal);
                        var end = start.Substring(0, endIndex);
                        value = end;
                        break;
                    }
                }

            return value;
        }
        public static string GetXForwardedFor(string GuestID)
        {
            try
            {
                var baseKey = "0e6be1f1e21ffc33590b888fd4dc81b19713e570e805d4e5df80a493c9571a05";
                var plainText = $"{{\"webgl_fingerprint\":\"\",\"canvas_fingerprint\":\"\",\"navigator_properties\":{{\"hasBeenActive\":\"false\",\"userAgent\":\"{TdConstants.NewUserAgent}\",\"webdriver\":\"false\"}},\"codec_fingerprint\":\"\",\"audio_fingerprint\":\"\",\"audio_properties\":null,\"created_at\":{DateTime.Now.GetCurrentEpochTimeSeconds()}}}";
                var g = new XPFFHeaderGenerator(baseKey);
                return g.GenerateXpff(plainText, GuestID);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static string GetAssignmentToken(string data, string paramName)
        {
            var value = string.Empty;
            var startIndx = data.IndexOf(paramName, StringComparison.Ordinal);
            if (startIndx > 0)
            {
                var endIndx = data.IndexOf("\"", startIndx, StringComparison.Ordinal);
                value = data.Substring(startIndx, endIndx - startIndx).Replace(",", "");
                if (value.Contains(paramName))
                    try
                    {
                        var getOuthentication = Regex.Split(data, "\"AssignmentToken\":\"");
                        var authenticity = Regex.Split(getOuthentication[1], ",");

                        if (authenticity[0].IndexOf("\"", StringComparison.Ordinal) > 0)
                        {
                            var indexStart1 = authenticity[0].IndexOf("\"", StringComparison.Ordinal);
                            var start = authenticity[0].Substring(0, indexStart1);
                            value = start.Replace("\"", "").Replace(":", "");
                        }
                    }
                    catch
                    {
                    }

                return value;
            }

            var array = Regex.Split(data, "<input type=\"hidden\"");
            foreach (var item in array)
                if (item.Contains("assignment_token"))
                {
                    var startindex = item.IndexOf("value=\"", StringComparison.Ordinal);
                    if (startindex > 0)
                    {
                        var start = item.Substring(startindex).Replace("value=\"", "");
                        var endIndex = start.IndexOf("\"", StringComparison.Ordinal);
                        var end = start.Substring(0, endIndex);
                        value = end;
                        break;
                    }
                }

            return value;
        }

        public static string GetLocationWiseSearchFormat(string query, out string RefererPattern)
        {
            try
            {
                var FormatPattern = string.Empty;
                var listDetail = query.Replace(",", ":").Split(':').ToList();
                var Keyword = listDetail.Count == 4 || listDetail.Count == 1 && !string.IsNullOrEmpty(listDetail[0].Trim())
                    ? listDetail[0].Trim()
                    : "";
                var Location1 = listDetail.Count == 3 || listDetail.Count == 2 && !string.IsNullOrEmpty(listDetail[0].Trim()) ? listDetail[0].Trim() : string.Empty;
                var Location2 = listDetail.Count == 3 || listDetail.Count == 2 && !string.IsNullOrEmpty(listDetail[1].Trim()) ? listDetail[1].Trim() : string.Empty;
                var Distance = listDetail.Count == 3 && !string.IsNullOrEmpty(listDetail[2].Trim())
                    ? listDetail[2].Trim()
                    : listDetail.Count == 4 && !string.IsNullOrEmpty(listDetail[3].Trim()) ?
                    listDetail[2].Trim() : string.Empty;
                Keyword = string.IsNullOrEmpty(Keyword) ? string.Empty : $"{Keyword} ";
                Location1 = string.IsNullOrEmpty(Location1) ?string.Empty:$"near:{Location1}";
                Location2 = string.IsNullOrEmpty(Location2) ? string.Empty : $",{Location2}";
                Distance = string.IsNullOrEmpty(Distance) ? string.Empty : $" within:{Distance}mi";
                FormatPattern = $"{Keyword}{Location1}{Location2}{Distance}";
                RefererPattern = $"{Keyword}{Location1}{Location2}{Distance}";
                RefererPattern = Uri.EscapeDataString(RefererPattern);
                return Uri.EscapeDataString(FormatPattern);
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
                RefererPattern = "";
                return "";
            }
        }

        public static long UnixTimestampFromDateTime(DateTime date)
        {
            var unixTimestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
            unixTimestamp /= TimeSpan.TicksPerSecond;
            return unixTimestamp;
        }

        public static string getUriMatrix(string response)
        {
            var uri_Matrix = "";
            var spilitData = Regex.Split(response, "var");
            try
            {
                spilitData = spilitData.Skip(2).ToArray();
            }
            catch (Exception)
            {
            }

            var forthValue = new List<string>();
            foreach (var loopspilitData in spilitData)
            {
                var temploopspilitData = loopspilitData;
                temploopspilitData = "-" + temploopspilitData;
                var tempData = Utilities.GetBetween(temploopspilitData, "-", "=");
                forthValue.Add(tempData);
                if (forthValue.Count == 4) break;
            }

            var lastvalue = Utilities.GetBetween(response, "'s':'", "'}");
            try
            {
                uri_Matrix = "{\"rf\":" + forthValue[0] + ":2," + forthValue[1] + ":1," + forthValue[2] + ":-1," +
                             forthValue[3] + ":1},\"s\":" + lastvalue + "}";
            }
            catch (Exception)
            {
            }

            return uri_Matrix;
        }

        public static string getUriJsonForSolveCaptcha(string response)
        {
            var uri_Matrix = "";
            var spilitData = Regex.Split(response, "var");
            try
            {
                spilitData = spilitData.Skip(2).ToArray();
            }
            catch (Exception)
            {
            }

            var forthValue = new List<string>();
            foreach (var loopspilitData in spilitData)
            {
                var temploopspilitData = loopspilitData;
                temploopspilitData = "-" + temploopspilitData;
                var tempData = Utilities.GetBetween(temploopspilitData, "-", "=");
                forthValue.Add(tempData);
                if (forthValue.Count == 4) break;
            }

            var lastvalue = Utilities.GetBetween(response, "'s':'", "'}");
            try
            {
                uri_Matrix = "{\"rf\":{\"" + forthValue[0]?.Trim() + "\":2,\"" + forthValue[1]?.Trim() + "\":1,\"" +
                             forthValue[2]?.Trim() + "\":-1,\"" +
                             forthValue[3]?.Trim() + "\":1},\"s\":" + lastvalue?.Trim() + "\"}";
            }
            catch (Exception)
            {
            }

            return uri_Matrix;
        }

        public static string GetRandomHexNumber(int digits)
        {
            var buffer = new byte[digits / 2];
            Random.NextBytes(buffer);
            var result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + Random.Next(16).ToString("X");
        }

        public static DateTime GetDateFormatFromString(string Date)
        {
            DateTime TimeStamp;
            try
            {
                var Date2 = Regex.Split(Date, "-")[1].Trim();
                Date2 = Date2.Replace(" ", "-");
                var dateTime = DateTime.Parse(Date2, CultureInfo.InvariantCulture);
                TimeStamp = dateTime;
            }
            catch (Exception)
            {
                return DateTime.Today;
            }

            return TimeStamp;
        }

        public static int GetDateDifferenceFromTimeStamp(int TimeStamp)
        {
            try
            {
                // here we taking tweets within day
                // therefore we using Math.Ceiling
                var currentDate = DateTime.UtcNow;
                var date1 = TimeStamp.EpochToDateTimeUtc();
                var ts = currentDate - date1;
                //  return (int)(ts.TotalDays);
                return (int)Math.Ceiling(ts.TotalDays);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static int GetHourDifferenceFromTimeStamp(DateTime DateTime)
        {
            try
            {
                var currentDate = DateTime.UtcNow;
                var ts = currentDate - DateTime;
                
                return (int)Math.Ceiling(ts.TotalHours);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static string EncodeBase64String(string Base64string)
        {
            try
            {
                var value = Base64string;
                var limit = 2000;

                var sb = new StringBuilder();
                var loops = value.Length / limit;

                for (var i = 0; i <= loops; i++)
                    if (i < loops)
                        sb.Append(Uri.EscapeDataString(value.Substring(limit * i, limit)));
                    else
                        sb.Append(Uri.EscapeDataString(value.Substring(limit * i)));
                return sb.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static List<string> DownloadFileFromTwitter(string FileUrl, string format, string FolderPath = null,
            string FileName = null)
        {
            var FileList = new List<string>();
            try
            {
                //Change it later
                var fileList = FileUrl.Split('\n').ToList();

                foreach(var file in fileList)
                {
                    var TempPath = string.Empty;
                    var FilePath = string.Empty;
                    if (file.Contains(".mp4"))
                        format = ".mp4";
                    else
                        format = ".jpg";
                    if (string.IsNullOrEmpty(FolderPath))
                        TempPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Socinator\\Twitter";
                    else
                        TempPath = FolderPath;
                    TempPath = TempPath + "\\" +
                               DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture).Replace("/", "-");
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var objwebclient = new WebClient();
                    var array = objwebclient.DownloadData(file);
                    var Title = string.Empty;

                    string[] areaay = { "A", "B", "C", "D", "E", "G" };
                    var rn = new Random();
                    Title = areaay[rn.Next(0, 5)] + "" + rn.Next(0, 1000000);

                    if (!string.IsNullOrEmpty(FileName)) Title = FileName + "-" + Title;
                    FilePath = TempPath + "\\" + Title + format;

                    if (!Directory.Exists(TempPath))
                    {
                        Directory.CreateDirectory(TempPath);
                        objwebclient.UploadData(FilePath, array);
                        FileList.Add(FilePath);
                    }
                    else
                    {
                        objwebclient.UploadData(FilePath, array);
                        FileList.Add(FilePath);
                    }
                }

                
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new List<string>();
            }

            return FileList;
        }

        public static bool CheckDuration(Uri Media)
        {
            var VideoFlag = false;

            try
            {
                var IsOpened = false;
                var IsFailed = false;

                var Player = new MediaPlayer();

                Player.MediaFailed += (s, e) => IsFailed = true;
                Player.MediaOpened += (s, e) => IsOpened = true;

                Player.ScrubbingEnabled = true;

                Player.Open(Media);

                var TotalDuration = Player.NaturalDuration.TimeSpan.TotalMilliseconds;
                if (TotalDuration > 140 * 1000)
                    return true;

                Player.Close();
            }
            catch (Exception ex)
            {
                if (Media != null)
                    ex.DebugLog($"Error UI Tweet Poster Module: Path {Media.AbsolutePath}");
            }

            return false;
        }

        public static string GetMediaTagFormat(string MediaId, List<string> ListUserId)
        {
            string result;
            try
            {
                var ListDictionary = new List<Dictionary<string, string>>();

                foreach (var UserId in ListUserId)
                {
                    var dictionary = new Dictionary<string, string>();
                    dictionary.Add("type", "user");
                    dictionary.Add("user_id", UserId);
                    ListDictionary.Add(dictionary);
                }

                var FinalDictionary = new Dictionary<string, List<Dictionary<string, string>>>();
                FinalDictionary.Add(MediaId, ListDictionary);
                result = JsonConvert.SerializeObject(FinalDictionary);
                result = Uri.EscapeDataString(result);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }

            return result;
        }

        public static string GetTweetIdFromUrl(string TweetUrl)
        {
            var TweetId = string.Empty;
            try
            {
                if (TweetUrl.Contains("https:"))
                    TweetId = Regex.Split(TweetUrl, "status/")[1].Trim();
                else
                    TweetId = TweetUrl;
            }
            catch (Exception)
            {
            }

            return TweetId;
        }

        public static string GetUserNameFromUrl(string Url)
        {
            try
            {
                if (Url.Contains(TdConstants.MainUrl))
                {
                    var SplitUrl = Regex.Split(Url, ".com/")[1];
                    return SplitUrl.Contains("status") ? Regex.Split(SplitUrl, "/status")[0] : SplitUrl;
                }

                return Url;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetVideoUrlFromThirdParty(string TweetId, string UserName, int MediaQuality)
        {
            try
            {
                var StatusUrl = TdConstants.MainUrl + UserName + "/status/" + TweetId;
                var DownloadUrl = string.Empty;
                var objThirdPartyRequest = new ExtraRequests();
                //http://twittervideodownloader.com/
                var Response = objThirdPartyRequest.PostRequest("https://twdownload.com/",
                    "https://twdownload.com/download-track/", "twitter-url=" + Uri.EscapeDataString(StatusUrl), true);
                if (!string.IsNullOrEmpty(Response))
                    if (Response.Contains(".mp4"))
                    {
                        var ListDownloadUrl =
                            HtmlAgilityHelper.getListValueWithAttributeNameFromClassName(Response,
                                "btn btn-primary btn-sm btn-block", "href");
                        DownloadUrl = MediaQuality == 0
                            ? ListDownloadUrl[ListDownloadUrl.Count - 2]
                            : ListDownloadUrl[0];

                        if (string.IsNullOrEmpty(DownloadUrl) || !DownloadUrl.Contains(".mp4"))
                            DownloadUrl = ListDownloadUrl[1];

                        if (string.IsNullOrEmpty(DownloadUrl) || !DownloadUrl.Contains(".mp4"))
                            DownloadUrl = "https://video.twimg.com/tweet_video/" +
                                          Utilities.GetBetween(Response, "video.twimg.com/tweet_video/", ".mp4") +
                                          ".mp4";

                        if (!string.IsNullOrEmpty(DownloadUrl) && DownloadUrl.Contains("?"))
                            DownloadUrl = DownloadUrl.Split('?').ToList()[0];

                        if (!string.IsNullOrEmpty(DownloadUrl) && DownloadUrl.Contains(".mp4"))
                            return DownloadUrl;
                    }

                Response = objThirdPartyRequest.PostRequest("https://twdown.net/", "https://twdown.net/download.php",
                    "URL=" + Uri.EscapeDataString(StatusUrl), false);
                if (!string.IsNullOrEmpty(Response))
                {
                    var VideoToken = GetTwtDownVideoToken(Response);
                    if (!string.IsNullOrEmpty(VideoToken) && VideoToken.Contains("pu/vid/"))

                        return $"https://video.twimg.com/ext_tw_video/{VideoToken}.mp4";
                    if(!string.IsNullOrEmpty(VideoToken))
                        return $"https://video.twimg.com/amplify_video/{VideoToken}.mp4";
                    return string.Empty;
                }


                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetTwtDownVideoToken(string TwtDownResponse)
        {
            var videoToken = "";
            try
            {
                // updated video url response
                if (string.IsNullOrEmpty(videoToken =
                    Utilities.GetBetween(TwtDownResponse, "video.twimg.com/tweet_video/", ".mp4")))
                    videoToken = Utilities.GetBetween(TwtDownResponse, "video.twimg.com/ext_tw_video/", ".mp4");
                if (string.IsNullOrEmpty(videoToken))
                {
                    if (!TwtDownResponse.Contains("?tag=13"))
                    {
                        videoToken = Utilities.GetBetween(TwtDownResponse, "<a download href=\"", "\"");
                        videoToken = Utilities.GetBetween(videoToken, "https://video.twimg.com/amplify_video/", ".mp4");
                    }
                    else if (!string.IsNullOrEmpty(videoToken =
                        Utilities.GetBetween(TwtDownResponse, "<a download href=\"", "?tag=13")))
                    {
                        videoToken = Utilities.GetBetween(videoToken, "https://video.twimg.com/amplify_video/", ".mp4");
                    }
                }
                else

                {
                    return videoToken;
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }

            return videoToken;
        }


        public static Dictionary<string, string> GetAllEmojis()
        {
            var Emojis = new Dictionary<string, string>();
            Emojis.Add("Grinning face", "😀");
            Emojis.Add("Grimacing face", "😬");
            Emojis.Add("Grinning face with smiling eyes", "😁");
            Emojis.Add("Face with tears of joy", "😂");
            Emojis.Add("Smiling face with open mouth", "😃");
            Emojis.Add("Smiling face with open mouth and smiling eyes", "😄");
            Emojis.Add("Rolling on the floor laughing", "🤣");
            Emojis.Add("Smiling face with open mouth and cold sweat", "😅");
            Emojis.Add("Smiling face with open mouth and tightly-closed eyes", "😆");
            Emojis.Add("Smiling face with halo", "😇");
            Emojis.Add("Winking face", "😉");
            Emojis.Add("Smiling face with smiling eyes", "😊");
            Emojis.Add("Slightly smiling face", "🙂");
            Emojis.Add("Upside-down face", "🙃");
            Emojis.Add("Smiling face", "☺️");
            Emojis.Add("Face savouring delicious food", "😋");
            Emojis.Add("Relieved face", "😌");
            Emojis.Add("Smiling face with heart-shaped eyes", "😍");
            Emojis.Add("Face throwing a kiss", "😘");
            Emojis.Add("Kissing face", "😗");
            Emojis.Add("Kissing face with smiling eyes", "😙");
            Emojis.Add("Kissing face with closed eyes", "😚");
            Emojis.Add("Crazy face", "🤪");
            Emojis.Add("Face with stuck-out tongue and winking eye", "😜");
            Emojis.Add("Face with stuck-out tongue and tightly-closed eyes", "😝");
            Emojis.Add("Face with stuck-out tongue", "😛");
            Emojis.Add("Money-mouth face", "🤑");
            Emojis.Add("Smiling face with sunglasses", "😎");
            Emojis.Add("Nerd face", "🤓");
            Emojis.Add("Face with monocle", "🧐");
            Emojis.Add("Face with cowboy hat", "🤠");
            Emojis.Add("Hugging face", "🤗");
            Emojis.Add("Clown face", "😏");
            Emojis.Add("Face without mouth", "😶");
            Emojis.Add("Neutral face", "😐");
            Emojis.Add("Expressionless face", "😑");
            Emojis.Add("Unamused face", "😒");
            Emojis.Add("Face with rolling eyes", "🙄");
            Emojis.Add("Face with raised eyebrow", "🤨");
            Emojis.Add("Thinking face", "🤔");
            Emojis.Add("Shushing face", "🤫");
            Emojis.Add("Face with hand over mouth", "🤭");
            Emojis.Add("Lying face", "🤥");
            Emojis.Add("Flushed face", "😳");
            Emojis.Add("Disappointed face", "😞");
            Emojis.Add("Worried face", "😟");
            Emojis.Add("Angry face", "😠");
            Emojis.Add("Pouting face", "😡");
            Emojis.Add("Face with symbols over mouth", "🤬");
            Emojis.Add("Pensive face", "😔");
            Emojis.Add("Confused face", "😕");
            Emojis.Add("Slightly frowning face", "🙁");
            Emojis.Add("Frowning face", "☹️");
            Emojis.Add("Persevering face", "😣");
            Emojis.Add("Confounded face", "😖");
            Emojis.Add("Tired face", "😫");
            Emojis.Add("Weary face", "😩");
            Emojis.Add("Face with look of triumph", "😤");
            Emojis.Add("Face with open mouth", "😮");
            Emojis.Add("Face screaming in fear", "😱");
            Emojis.Add("Fearful face", "😨");
            Emojis.Add("Face with open mouth and cold sweat", "😰");
            Emojis.Add("Hushed face", "😯");
            Emojis.Add("Frowning face with open mouth", "😦");
            Emojis.Add("Anguished face", "😧");
            Emojis.Add("Crying face", "😢");
            Emojis.Add("Disappointed but relieved face", "😥");
            Emojis.Add("Sleepy face", "😪");
            Emojis.Add("Drooling face", "🤤");
            Emojis.Add("Face with cold sweat", "😓");
            Emojis.Add("Loudly crying face", "😭");
            Emojis.Add("Star-struck", "🤩");
            Emojis.Add("Dizzy face", "😵");
            Emojis.Add("Astonished face", "😲");
            Emojis.Add("Exploding head", "🤯");
            Emojis.Add("Zipper-mouth face", "🤐");
            Emojis.Add("Face with medical mask", "😷");
            Emojis.Add("Face with head-bandage", "🤕");
            Emojis.Add("Face with thermometer", "🤒");
            Emojis.Add("Face vomiting", "🤮");
            Emojis.Add("Nauseated face", "🤢");
            Emojis.Add("Sneezing face", "🤧");
            Emojis.Add("Sleeping face", "😴");
            Emojis.Add("Kiss mark", "💋");
            Emojis.Add("Man dancing", "🕺");
            Emojis.Add("Couple with heart", "💑");
            Emojis.Add("Couple with heart (woman, woman)", "👩‍❤️‍👩");
            Emojis.Add("Kiss", "💏");
            Emojis.Add("Kiss (woman, woman)", "👩‍❤️‍💋‍👩");
            Emojis.Add("Heavy red heart", "❤️");
            Emojis.Add("Heart suit", "♥️");
            Emojis.Add("Revolving hearts", "💞");
            Emojis.Add("Growing heart", "💗");
            Emojis.Add("Sparkling heart", "💖");
            Emojis.Add("Broken heart", "💔");
            Emojis.Add("Heart with arrow", "💘");
            Emojis.Add("Blue heart", "💙");
            Emojis.Add("Green heart", "💚");
            Emojis.Add("Purple heart", "💜");
            Emojis.Add("Orange heart", "🧡");
            Emojis.Add("Yellow heart", "💛");
            Emojis.Add("Person raising both hands in celebration", "🙌");
            Emojis.Add("Person with folded hands", "🙏");
            Emojis.Add("Thumbs up sign", "👍");
            Emojis.Add("Ok hand sign", "👌");
            Emojis.Add("Sign of the horns", "🤘");
            Emojis.Add("Flexed biceps", "💪");
            Emojis.Add("Hand with index and middle fingers crossed", "🤞");
            Emojis.Add("Thumbs down sign", "👎");
            Emojis.Add("Clapping hands sign", "👏");
            Emojis.Add("Handshake", "🤝");
            Emojis.Add("Dancer", "💃");
            Emojis.Add("Sparkles", "✨");
            Emojis.Add("Pushpin", "📌");
            Emojis.Add("Fire", "🔥");
            Emojis.Add("Woman tipping hand", "💁‍♀️");
            Emojis.Add("Tongue", "👅");
            Emojis.Add("Mouth", "👄");
            Emojis.Add("Eye", "👀");
            Emojis.Add("Eyes", "👁️");
            return Emojis;
        }

        public static string ReplaceImageWithEmogis(string text, Dictionary<string, string> EmojiDict)
        {
            try
            {
                var ReplacedText = text;
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(ReplacedText);
                var nodes = htmlDoc.DocumentNode.SelectNodes("//img[@class='Emoji Emoji--forText']");
                if (nodes != null)
                    foreach (var node in nodes)
                    {
                        var htmlTag = node.OuterHtml;
                        var EmojiTitle = Utilities.GetBetween(htmlTag, "title=\"", "\"");
                        string emojiValue;

                        if (EmojiDict.ContainsKey(EmojiTitle))
                        {
                            emojiValue = EmojiDict[EmojiTitle];
                        }
                        else
                        {
                            var values = EmojiDict.Values.ToList();
                            emojiValue = values[Random.Next(values.Count - 1)];
                        }

                        ReplacedText = Regex.Replace(ReplacedText, htmlTag, emojiValue);
                    }

                ReplacedText =
                    HtmlAgilityHelper.getStringInnerTextFromClassName(ReplacedText, "js-tweet-text tweet-text");
                return ReplacedText;
            }
            catch (Exception)
            {
                return HtmlAgilityHelper.getStringInnerHtmlFromClassName(text, "js-tweet-text tweet-text");
            }
        }

        public static bool IsZeroOrEmpty(string data)
        {
            try
            {
                return string.IsNullOrEmpty(data?.Trim()) || "0".Equals(data.Trim());
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        public static string GetTweetOrUserId(string jsonResponse, string varName = null, string varName2 = null,
            bool tweetTrue = false)
        {
            var tweetOrUserId = "tweetOrUserId";
            try
            {
                if (string.IsNullOrEmpty(varName))
                    varName = "id_str";
                if (jsonResponse.Contains("<!DOCTYPE html>") && tweetTrue)
                    tweetOrUserId = Utilities.GetBetween(jsonResponse, "data-max-position=\"", "\"");
                if (string.IsNullOrEmpty(tweetOrUserId))
                {
                    tweetOrUserId = ForGetborwserTweetId(jsonResponse);
                    if (string.IsNullOrEmpty(tweetOrUserId))
                        tweetOrUserId = "tweetOrUserId";
                }
                else if (jsonResponse.Contains("<!DOCTYPE html>"))
                {
                    var messagesHtmlDoc = new HtmlDocument();
                    messagesHtmlDoc.LoadHtml(jsonResponse);
                    var listMessages =
                        HtmlAgilityHelper.getListInnerHtmlFromClassName(jsonResponse, "stream-item-header",
                            messagesHtmlDoc);
                    if (listMessages.Count() == 0)
                        tweetOrUserId = "tweetOrUserId";
                    else
                        tweetOrUserId = Utilities.GetBetween(listMessages[0], "data-user-id=\"", "\"");
                }
                else if (jsonResponse.Contains("{\"created_at\":"))
                {
                    tweetOrUserId = string.IsNullOrEmpty(varName2)
                        ? Utilities.GetBetween(jsonResponse, "\"rest_id\":\"", "\"")
                        : Utilities.GetBetween(jsonResponse, "\"id_str\":\"", "\"");
                }
                else
                {
                    tweetOrUserId = Utilities.GetBetween(jsonResponse, "\"rest_id\":\"", "\"");
                    tweetOrUserId = string.IsNullOrEmpty(tweetOrUserId)? Utilities.GetBetween(jsonResponse, "\"topic\":\"/tweet_engagement/", "\""):tweetOrUserId;
                    if (string.IsNullOrEmpty(tweetOrUserId))
                        tweetOrUserId = "tweetOrUserId";
                }
            }
            catch (Exception exception)
            {
                exception.DebugLog();
                var id = Utilities.GetBetween(jsonResponse, "\"rest_id\":\"", "\"");
                if(!string.IsNullOrEmpty(id))
                    tweetOrUserId = id;
            }

            return tweetOrUserId;
        }

        private static string ForGetborwserTweetId(string jsonResponse)
        {
            string tweetOrUserId;
            var now = DateTime.Now;
            var date = Convert.ToDateTime(now);
            var ss = date.ToString();
            var spildate = Regex.Split(ss, " ");
            var splittime = Regex.Split(spildate[1], ":");
            var timeformate = $"{splittime[0]}:{splittime[1]} {spildate[2]}";
            var splitdatewithmonth = Regex.Split(spildate[0], "-");
            var numberofmonth = int.Parse(splitdatewithmonth[1]);
            var month = (Enums.Month)numberofmonth;
            var monthformate = $"{month} {splitdatewithmonth[0]}, {splitdatewithmonth[2]}";
            tweetOrUserId = Utilities.GetBetween(jsonResponse, $"<a title=\"{timeformate} · {monthformate}\" href=\"",
                "\"");
            tweetOrUserId = Utilities.GetBetween($"{tweetOrUserId}\"", "/status/", "\"");
            return tweetOrUserId;
        }

        public static void SaveUserProfileDetails(DominatorAccountModel dominatorAccountModel,
            IAccountContactConfig _accountContactConfig = null)
        {
            try
            {
                if (_accountContactConfig == null)
                    _accountContactConfig = InstanceProvider.GetInstance<IAccountContactConfig>();

                var UserProfileDetails =
                    _accountContactConfig.GetUserContactDetails(dominatorAccountModel, true).Result;
                var serializedUserProfileDetails = JsonConvert.SerializeObject(UserProfileDetails);
                SocinatorAccountBuilder.Instance(dominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateExtraParameter(Enums.ModuleExtraDetails.UserProfileDetails.ToString(),
                        serializedUserProfileDetails)
                    .SaveToBinFile();
                // UserProfileDetails userContactDetails = AccountContactConfig.GetUserContactDetails(dominatorAccountModel).Result;
                lock (LockFile)
                {
                    _accountContactConfig.SaveUserContactDetails(UserProfileDetails);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static CookieCollection GetCookieCollectionFromEmbeddedBrowser(BrowserWindow browserWindow,
            out List<Cookie> lstCookies)
        {
            lstCookies = browserWindow.Browser.RequestContext.GetCookieManager(new TaskCompletionCallback())
                .VisitAllCookiesAsync().Result;

            var cookieCollection = new CookieCollection();

            foreach (var item in lstCookies)
                try
                {
                    var cookie = new System.Net.Cookie
                    {
                        Name = item.Name,
                        Value = item.Value,
                        Domain = item.Domain,
                        Path = item.Path,
                        Secure = item.Secure
                    };
                    if (item.Expires != null)
                        cookie.Expires = (DateTime)item.Expires;
                    cookieCollection.Add(cookie);
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.StackTrace);
                }

            return cookieCollection;
        }

        public static void StopActivity(string accountId, ActivityType activityType)
        {
            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
            dominatorScheduler.ChangeAccountsRunningStatus(false, accountId, activityType);
        }

        public static bool IsContains(string pageSource, params string[] containsList)
        {
            foreach (var contain in containsList)
                if (pageSource.Contains(contain))
                    return true;

            return false;
        }

        public static int ConvertDoubleAndInt(string input)
        {
            var doubleResult = Convert.ToDouble(input);
            return Convert.ToInt32(doubleResult);
        }

        public static string GetResponseHandlerWithkeys(
            List<KeyValuePair<string, MemoryStreamResponseFilter>> ListOfResponse, string Keys)
        {
            var response = string.Empty;
            try
            {

                ListOfResponse.Where(x => x.Value.Data == null).ForEach(x => x.Value?.Dispose());
                ListOfResponse.RemoveAll(x => x.Value.Data == null);
                var responseStream = ListOfResponse.Where(x => x.Key == Keys).Select(x => x.Value).ToList();
                responseStream.Where(x => x.Data == null).ForEach(x => x?.Dispose());
                responseStream.RemoveAll(x => x.Data == null);
                response = Encoding.UTF8.GetString(responseStream[0].Data);
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static string GetResponseHandlerWithkeysContain(
            List<KeyValuePair<string, MemoryStreamResponseFilter>> ListOfResponse, string Keys, bool isLast = false,
            string secondarykeys = "")

        {
            var response = string.Empty;
            try
            {
                var keyvalue = new List<KeyValuePair<string, string>>();
                ListOfResponse.Where(x => x.Value.Data == null).ForEach(x => x.Value?.Dispose());
                ListOfResponse.RemoveAll(x => x.Value.Data == null);
                List<MemoryStreamResponseFilter> responseStream;
                if (string.IsNullOrEmpty(secondarykeys))
                    responseStream = ListOfResponse.Where(x => x.Key.Contains(Keys)).Select(x => x.Value).ToList();
                else
                    responseStream = ListOfResponse.Where(x => x.Key.Contains(Keys) && x.Key.Contains(secondarykeys))
                        .Select(x => x.Value).ToList();
                if (string.IsNullOrEmpty(secondarykeys))
                    keyvalue.AddRange(ListOfResponse.Where(x => x.Key.Contains(Keys)).Select(x =>
                        new KeyValuePair<string, string>(x.Key, Encoding.UTF8.GetString(x.Value.Data))).ToList());
                else
                    keyvalue.AddRange(ListOfResponse.Where(x => x.Key.Contains(Keys) && x.Key.Contains(secondarykeys))
                        .Select(x => new KeyValuePair<string, string>(x.Key, Encoding.UTF8.GetString(x.Value.Data)))
                        .ToList());
                for (var index = 0; index < responseStream.Count; index++)
                {
                    response = Encoding.UTF8.GetString(responseStream[index].Data);
                    if (!IsEncrypted(response))
                        break;
                }

                if (isLast)
                    response = Encoding.UTF8.GetString(responseStream.Last().Data);
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool IsEncrypted(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return text.StartsWith("GIF89a", StringComparison.InvariantCulture);
        }

        public static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (var stream = new GZipStream(new MemoryStream(gzip),
                CompressionMode.Decompress))
            {
                const int size = 4096;
                var buffer = new byte[size];
                using (var memory = new MemoryStream())
                {
                    var count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0) memory.Write(buffer, 0, count);
                    } while (count > 0);

                    return memory.ToArray();
                }
            }
        }

        public static string DecompressResponse(byte[] gzip)
        {
            var decompressedResponse = "";
            try
            {
                using (var stream = new GZipStream(new MemoryStream(Compress(gzip)),
                    CompressionMode.Decompress))
                {
                    const int size = 4096;
                    var buffer = new byte[size];
                    using (var memory = new MemoryStream())
                    {
                        int count;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0) memory.Write(buffer, 0, count);
                        } while (count > 0);

                        decompressedResponse = Encoding.UTF8.GetString(memory.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return decompressedResponse;
        }


        private static byte[] Compress(byte[] data)
        {
            var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress, true))
            {
                gzip.Write(data, 0, data.Length);
                gzip.Close();
            }

            return output.ToArray();
        }

        public static List<KeyValuePair<string, MemoryStreamResponseFilter>> GetResponseData(
            BrowserWindow BrowserWindow)
        {
            var delayService = InstanceProvider.GetInstance<IDelayService>();
            List<KeyValuePair<string, MemoryStreamResponseFilter>> Responsedata;
            var i = 0;

            do
            {
                i++;

                Responsedata = BrowserWindow.TwitterJsonResponse();
                delayService.ThreadSleep(2000);
            } while (Responsedata == null && i <= 2);


            return Responsedata;
        }

        public static string GetResponse(List<KeyValuePair<string, MemoryStreamResponseFilter>> responseData,
            params string[] keyParams)
        {
            var response = "";
            for (var index = 0; index < keyParams.Length && string.IsNullOrWhiteSpace(response); index++)
                response = GetResponseHandlerWithkeysContain(responseData, keyParams[index]);

            return response;
        }


        public static string GetResponseData(BrowserWindow BrowserWindow, string key, bool isLast = false)
        {
            var responseData = new List<KeyValuePair<string, MemoryStreamResponseFilter>>();
            try
            {

                responseData = GetResponseData(BrowserWindow);
                var responseWithKeys = GetResponseHandlerWithkeysContain(responseData, key, isLast);
                return responseWithKeys;
            }
            catch (Exception)
            {
                responseData?.ForEach(x => x.Value?.Dispose());
                throw;
            }
            finally
            {
                responseData?.ForEach(x => x.Value?.Dispose());
            }
        }

        public static string GetTaggedUserPostData(List<string> UserIDS)
        {
            string postData = string.Empty;
            if (UserIDS.Count <= 0)
                return postData;
            else
            {
                foreach(var id in UserIDS)
                {
                    if (id != UserIDS.LastOrDefault())
                        postData += $"\"{id}\",";
                    else
                        postData += $"\"{id}\"";
                }
                return postData;
            }
        }
        public static string GetMediaPostData(List<string> MediaIDS,List<string> UserIDS)
        {
            string postData = string.Empty;
            if (MediaIDS.Count <= 0)
                return postData;
            else
            {
                foreach(var id in MediaIDS)
                {
                    if (id != MediaIDS.LastOrDefault())
                        postData += $"{{\"media_id\":\"{id}\",\"tagged_users\":[{GetTaggedUserPostData(UserIDS)}]}},";
                    else
                        postData += $"{{\"media_id\":\"{id}\",\"tagged_users\":[{GetTaggedUserPostData(UserIDS)}]}}";
                }
                return postData;
            }
        }
        public static string RemoveHtmlTags(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return Regex.Replace(input, "<.*?>", "");
        }
        public static DateTime GetDateTime(string Created)
        {
            try
            {
                if(string.IsNullOrEmpty(Created)) return DateTime.Now;
                var splitted = Created.Split(' ');//Tue Sep 30 05:07:41 +0000 2008
                var date = $"{splitted[0]}, {splitted[2]} {splitted[1]} {splitted[5]} {splitted[3]}";
                DateTime.TryParse(date, out DateTime joinedDate);
                return joinedDate;
            }
            catch(Exception ex)
            {
                return DateTime.Now;
            }
        }
        public static string GetProfileDetails(string PageResponse)
        {
            var jsonResponse = string.Empty;
            try
            {
                jsonResponse = Utilities.GetBetween(PageResponse, "window.__INITIAL_STATE__=", ";window.__META_DATA__");
            }catch(Exception) { }
            return jsonResponse;
        }
        public static string GetTweetUrl(string Username,string tweetId)
        {
            if (string.IsNullOrEmpty(Username))
                return tweetId;
            return TdConstants.MainUrl + Username + "/status/" + tweetId;
        }
        public static string GetProfileUrl(string Username)
        {
            if (string.IsNullOrEmpty(Username))
                return Username;
            return TdConstants.MainUrl + Username;
        }
        public static string GetSpinTaxMessage(string Message)
        {
            if (string.IsNullOrEmpty(Message))
                return Message;
            try
            {
                var regexToMatchArabic = new Regex(@"[\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF\uFB50-\uFDFF\uFE70-\uFEFF]", RegexOptions.RightToLeft);
                var IsArabic = regexToMatchArabic.IsMatch(Message);
                if (!IsArabic)
                    return SpinTexHelper.GetSpinText(Message);
                else
                {
                    var arabicSpintaxRegex = new Regex(@"\(([^()])*\)", RegexOptions.RightToLeft);
                    var MatchCollection = arabicSpintaxRegex.Matches(Message);
                    foreach (Match match in MatchCollection)
                    {
                        var Spintax = match.Value?.Split('|').ToList()?.GetRandomItem()?.Replace("(","")?.Replace(")","");
                        Message = Message.Replace(match.Value, Spintax);
                    }
                    return Message;
                }
            }
            catch (Exception)
            {
                return Message;
            }
        }
    }
}