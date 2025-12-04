using System;
using System.Collections;

namespace LinkedDominatorCore.LDUtility
{
    public class RandomNumberGenerator
    {
        public int GenerateRandom(int minValue, int maxValue)
        {
            if (minValue <= maxValue)
            {
                var no = new Random().Next(minValue, maxValue);
                return no;
            }

            return 0;
        }

        public ArrayList RandomNumbers(int max)
        {
            // Create an ArrayList object that will hold the numbers
            var lstNumbers = new ArrayList();
            // The Random class will be used to generate numbers
            var rndNumber = new Random();

            // Generate a random number between 1 and the Max
            var number = rndNumber.Next(1, max + 1);
            // Add this first random number to the list
            lstNumbers.Add(number);
            // Set a count of numbers to 0 to start
            var count = 0;

            do // Repeatedly...
            {
                // ... generate a random number between 1 and the Max
                number = rndNumber.Next(1, max + 1);

                // If the newly generated number in not yet in the list...
                if (!lstNumbers.Contains(number))
                    // ... add it
                    lstNumbers.Add(number);


                count++;
            } while (count <= 10 * max); // Do that again


            return lstNumbers;
        }
    }
}