using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Image_Processing_X_Ray.DataStructures;

namespace Image_Processing_X_Ray.Algorithms
{
    class RegionFinding
    {
        #region Methods

        public static Matrix MarkRegions(Matrix matrix,out SetList equivalentRegions)
        {
            Matrix regionMatrix = new Matrix(matrix.Width, matrix.Height);
            equivalentRegions = new SetList();
            //equivalentRegions = null;
            //List<Cell> labeling = new List<Cell>();
            //Step 1
            int currentRegion = 1;
            //0 black , 255 white
            for(int y=0;y<matrix.Height;y++)
            {
                for(int x=0;x<matrix.Width;x++)
                {
                    if(matrix.Values[x,y]==1)
                    {
                        //Step 2
                        List<Cell> neighbors = matrix.GetNeighbors(x, y);
                        int matchCount = neighbors.Count(cell => cell.Value > 0);
                        //Step 3
                        if(matchCount==0)
                        {
                            regionMatrix.Values[x, y] = currentRegion;
                            //labeling.Add(new Cell(x,y,currentRegion));
                            equivalentRegions.Add(currentRegion, new Set() { currentRegion });
                            currentRegion += 1;
                        }
                        else if(matchCount==1)//Step 4
                        {
                            //Gan dung nhan ma no nhin thay
                            Cell oneCell = neighbors.First(cell => cell.Value == 1);
                            regionMatrix.Values[x, y] = regionMatrix.Values[oneCell.X, oneCell.Y];
                            //labeling.Add(new Cell(x, y, regionMatrix.Values[oneCell.X, oneCell.Y]));
                        }
                        else if(matchCount>1)
                        {
                            //List<int> distincRegions=neighbors.Select(cell => regionMatrix.Values[cell.X,cell.Y]).Distinct().ToList();
                            //Set distincRegions = new Set();
                            List<int> distincRegions = neighbors.Select(cell => regionMatrix.Values[cell.X, cell.Y]).Distinct().ToList();
                            
                            
                            while (distincRegions.Remove(0));
                            
                            if(distincRegions.Count==0)
                            {
                                regionMatrix.Values[x, y] = currentRegion;
                                equivalentRegions.Add(currentRegion, new Set() { currentRegion });
                                currentRegion+=1;
                            }
                            else if(distincRegions.Count==1)//step 5
                            {
                                regionMatrix.Values[x, y] = distincRegions[0];
                            }
                            else if(distincRegions.Count>1) 
                            {
                                int firstRegion = distincRegions[0];
                                regionMatrix.Values[x, y] = firstRegion;

                                //save equivalent give it into setlist

                                if(!equivalentRegions.ContainsKey(firstRegion))
                                {
                                    //equivalentRegions.Add(firstRegion, new Set {distincRegions });
                                    Set distincRegionsSet = new Set();
                                    distincRegionsSet.Clear();
                                    distincRegionsSet.AddRange(distincRegions);
                                    equivalentRegions.Add(firstRegion,distincRegionsSet);
                                   
                                }
                                else 
                                {
                                    foreach (int region in distincRegions)
                                    {
                                        if (!equivalentRegions[firstRegion].Contains(region))
                                        {
                                            equivalentRegions[firstRegion].Add(region);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            return regionMatrix;
        }
        public static Matrix MergeRegions(Matrix regionMatrix, SetList equivalentRegions, out Vector newRegionSets, out List<int> regionNumbers)
        {
            //Merge Region
            UnionFind unionFind = new UnionFind(equivalentRegions);
            regionNumbers = new List<int>();

            regionNumbers.AddRange(unionFind.Roots.Values);
            newRegionSets = new Vector();

            if(regionNumbers.Count>0)
            {
                int index = 1;
                int max = regionNumbers.Max();
                while(index <= max && regionNumbers.Count > 0)
                {
                    int min = regionNumbers.Min();
                    newRegionSets.Add(min, index);
                    index += 1;

                    while (regionNumbers.Remove(min)) ;
                }

            }

            for(int y=0;y<regionMatrix.Height;y++)
            {
                for(int x=0;x<regionMatrix.Width;x++)
                {
                    if (regionMatrix.Values[x,y] > 0)
                    {
                        regionMatrix.Values[x, y] = newRegionSets[unionFind.Roots[regionMatrix.Values[x, y]]];
                    }
                }
            }
            return regionMatrix;



        }
        #endregion
    }
}
