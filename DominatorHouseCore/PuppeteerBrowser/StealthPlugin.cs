using PuppeteerSharp;
using System.Threading.Tasks;

namespace DominatorHouseCore.PuppeteerBrowser
{
    public static class StealthPlugin
    {
        public static string ScrollBarCss = @"
                    /* ========== Watercolor Scrollbar ========== */
                    ::-webkit-scrollbar {
                        width: 14px;
                        height: 14px;
                    }

                    /* Watercolor-style track */
                    ::-webkit-scrollbar-track {
                        background: linear-gradient(180deg,
                        rgba(255,255,255,0.8) 0%,
                        rgba(225,245,255,0.7) 25%,
                        rgba(190,230,255,0.6) 50%,
                        rgba(225,245,255,0.7) 75%,
                        rgba(255,255,255,0.8) 100%);
                        background-size: 200% 200%;
                        border-radius: 10px;
                        animation: watercolorFlow 10s ease-in-out infinite;
                        box-shadow: inset 0 0 10px rgba(0,100,255,0.1);
                        backdrop-filter: blur(6px);
                    }

                    /* Semi-glass glowing thumb */
                    ::-webkit-scrollbar-thumb {
                        background: linear-gradient(180deg, rgba(0,140,255,0.75), rgba(0,190,255,0.65));
                        border-radius: 10px;
                        border: 2px solid rgba(255,255,255,0.5);
                        box-shadow: 0 0 8px rgba(0,150,255,0.4), inset 0 0 2px rgba(255,255,255,0.6);
                        transition: all 0.3s ease;
                    }

                    /* Hover effect */
                    ::-webkit-scrollbar-thumb:hover {
                        background: linear-gradient(180deg, rgba(0,180,255,0.9), rgba(0,220,255,0.8));
                        box-shadow: 0 0 12px rgba(0,200,255,0.6), inset 0 0 4px rgba(255,255,255,0.8);
                    }

                    /* Animation for moving watercolor waves */
                    @keyframes watercolorFlow {
                        0%   { background-position: 0% 50%; }
                        50%  { background-position: 100% 50%; }
                        100% { background-position: 0% 50%; }
                    }

                    /* Firefox fallback */
                    * {
                        scrollbar-width: thin;
                        scrollbar-color: rgba(0,180,255,0.7) rgba(220,240,255,0.8);
                    }";

        public static string VideoCodecJs = @"
                                    (function() {
                                            var s = document.createElement('script');
                                            s.src = 'https://cdn.jsdelivr.net/npm/ffmpeg@0.0.4/lib/ffmpeg.min.js';
                                            s.onload = function () {
                                            if (typeof FFmpeg !== 'undefined') {
                                                console.log('FFmpeg loaded, calling load()');
                                                FFmpeg.load();
                                            } else {
                                                console.error('FFmpeg not found!');
                                            }
                                    };
                                document.head.appendChild(s);
                            })();
                        ";
        public static async Task ApplyAsync(IPage page)
        {
            await page.EvaluateFunctionOnNewDocumentAsync(@"() => {
            // === navigator.webdriver ===
            Object.defineProperty(navigator, 'webdriver', {
                get: () => undefined
            });

            // === window.chrome ===
            window.chrome = {
                runtime: {}
            };

            // === navigator.languages ===
            Object.defineProperty(navigator, 'languages', {
                get: () => ['en-US', 'en']
            });

            // === navigator.plugins ===
            Object.defineProperty(navigator, 'plugins', {
                get: () => [1, 2, 3, 4, 5]
            });

            // === Permissions spoofing ===
            const originalQuery = window.navigator.permissions.query;
            window.navigator.permissions.query = function(parameters) {
                if (parameters.name === 'notifications') {
                    return Promise.resolve({ state: 'granted' });
                }
                return originalQuery.call(this, parameters);
            };

            Object.defineProperty(Notification, 'permission', {
                get: () => 'granted'
            });

            // === WebGL spoofing (WebGL, WebGL2, OffscreenCanvas) ===
            const spoofWebGL = (proto) => {
                const originalGetParameter = proto.getParameter;
                proto.getParameter = function(parameter) {
                    if (parameter === 37445) return 'NVIDIA Corporation'; // UNMASKED_VENDOR_WEBGL
                    if (parameter === 37446) return 'NVIDIA GeForce RTX 3080/PCIe/SSE2'; // UNMASKED_RENDERER_WEBGL
                    return originalGetParameter.call(this, parameter);
                };
            };

            if (typeof WebGLRenderingContext !== 'undefined') {
                spoofWebGL(WebGLRenderingContext.prototype);
            }

            if (typeof WebGL2RenderingContext !== 'undefined') {
                spoofWebGL(WebGL2RenderingContext.prototype);
            }

            try {
                const canvas = document.createElement('canvas');
                const gl = canvas.getContext('webgl') || canvas.getContext('webgl2');
                if (gl) {
                    spoofWebGL(Object.getPrototypeOf(gl));
                }
            } catch (e) {}

            try {
                if (typeof OffscreenCanvas !== 'undefined') {
                    const offscreen = new OffscreenCanvas(256, 256);
                    const gl = offscreen.getContext('webgl') || offscreen.getContext('webgl2');
                    if (gl) {
                        spoofWebGL(Object.getPrototypeOf(gl));
                    }
                }
            } catch (e) {}
        }");
        }
    }
}
