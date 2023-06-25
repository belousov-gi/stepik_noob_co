using System;
using System.Collections.Generic;
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

public class MainClass
{
    public static void Main()
    {
        string inputSpting;
        string[] inputSptingParams;
        List<Panda> infoOfAllPandas = new List<Panda>();
        List<Panda> infoOfAllPandasWithoutChanges = new List<Panda>();
        
        List<PandaPair> pandaPairs = new List<PandaPair>();
        inputSpting = Console.ReadLine();
        int idCounter = 0;
        
        while (inputSpting != "end")
        {
            if (inputSpting != null)
            {
                inputSptingParams = inputSpting.Split(' ');
                Panda panda = new Panda(Int32.Parse(inputSptingParams[0]), Int32.Parse(inputSptingParams[1]), inputSptingParams[2], idCounter);
                infoOfAllPandas.Add(panda);
                idCounter++;
            }
            inputSpting = Console.ReadLine();
        }
        
        foreach (var panda in infoOfAllPandas)
        {
            infoOfAllPandasWithoutChanges.Add(panda);
        }

        double minimalDistanceSquared;
        double distanceSquared;
        int idFirstPanda = -1;
        int idSecondPanda = -1;
        bool isPairFounded = true;

        while (isPairFounded)
        {
            isPairFounded = false;
            minimalDistanceSquared = double.PositiveInfinity;
            for (int i = 0; i < infoOfAllPandas.Count; i++)
            {
                for (int j = i + 1; j < infoOfAllPandas.Count; j++)
                {           
                  if (infoOfAllPandas[i].Sex != infoOfAllPandas[j].Sex) 
                    {
                        distanceSquared = Math.Pow((infoOfAllPandas[i].X - infoOfAllPandas[j].X), 2) +
                                          Math.Pow((infoOfAllPandas[i].Y - infoOfAllPandas[j].Y), 2);

                        if (distanceSquared < minimalDistanceSquared)
                        {
                            minimalDistanceSquared = distanceSquared;
                            idFirstPanda = infoOfAllPandas[i].Id;
                            idSecondPanda = infoOfAllPandas[j].Id;
                            isPairFounded = true;
                        } 
                    }
                }
            }
            
            if (isPairFounded)
            {
                decimal minimalDistance = (decimal)Math.Round(Math.Sqrt(minimalDistanceSquared), 2);
                PandaPair pandaPair = new PandaPair(idFirstPanda, idSecondPanda, minimalDistance, infoOfAllPandasWithoutChanges);
                pandaPairs.Add(pandaPair);
                infoOfAllPandas.RemoveAll(x => x.Id == idFirstPanda || x.Id == idSecondPanda);
            }
        }
        
        Console.WriteLine($"Total pandas count: {infoOfAllPandasWithoutChanges.Count}");
        
        foreach (var alonePanda in infoOfAllPandas)
        {
            Console.WriteLine($"Lonely {alonePanda.Sex} panda at X: {alonePanda.X}, Y: {alonePanda.Y}");
        }

        foreach (var pairOfPandas in pandaPairs)
        {
            Console.WriteLine(
                $"Pandas pair at distance {pairOfPandas.DistanceOfLove}, " +
                $"male panda at X: {pairOfPandas.XMalePanda}, Y: {pairOfPandas.YMalePanda}, " +
                $"female panda at X: {pairOfPandas.XFemalePanda}, Y: {pairOfPandas.YFemalePanda}");
        }        
    }
    
}
public class Panda
{
    public string Sex { get; }
    public int X { get; }
    public int Y { get; }
    public int Id { get; }
    public Panda(int x, int y, string sex, int idCounter)
    {
        Sex = sex;
        X = x;
        Y = y;
        Id = idCounter;
    }
}

public class PandaPair
{
    public int XMalePanda { get; }
    public int YMalePanda { get; }
    public int XFemalePanda { get; }
    public int YFemalePanda { get; }
    public decimal DistanceOfLove { get; }
    
    public PandaPair(int idFirstPanda, int idSecondPanda, decimal distanceOfLove, List<Panda> infoOfAllPandas)
    {
        DistanceOfLove = distanceOfLove;

        if (infoOfAllPandas[idFirstPanda].Sex == "male")
        {
            XMalePanda = infoOfAllPandas[idFirstPanda].X;
            YMalePanda = infoOfAllPandas[idFirstPanda].Y;
            
            XFemalePanda = infoOfAllPandas[idSecondPanda].X;
            YFemalePanda = infoOfAllPandas[idSecondPanda].Y;
            
        }
        else
        {
            XMalePanda = infoOfAllPandas[idSecondPanda].X;
            YMalePanda = infoOfAllPandas[idSecondPanda].Y;
            
            XFemalePanda = infoOfAllPandas[idFirstPanda].X;
            YFemalePanda = infoOfAllPandas[idFirstPanda].Y;
        }
    }
}



