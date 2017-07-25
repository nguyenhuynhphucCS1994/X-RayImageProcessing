using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Image_Processing_X_Ray.DataStructures;   

namespace Image_Processing_X_Ray.Algorithms
{
    class UnionFind
    {
        //private SetList equivalentRegions;

        
        #region Properties
        //Sets===Dictionary<int,Set>
        public SetList Sets { set; get; }
        //Roots===Dictionary<int,int>
        public Vector Roots { set; get; }

        #endregion

        #region Constructors
        public UnionFind(SetList sets)
        {
            this.Sets = sets;
            this.Roots = new Vector();

            this.Initialize();
            this.Start();
        }

        #endregion
        #region Methods

        public void Initialize()
        {
            //Initialize value of Sets is -1 
            Roots.Clear();
            foreach(int index in Sets.Keys)
            {
                foreach(int item in Sets[index])
                {
                    if(!Roots.ContainsKey(item))
                    {
                        Roots.Add(item, -1);
                    }
                }
            }

            //Assign value in Sets into Roots
            foreach(int index in Sets.Keys)
            {
                foreach(int item in Sets[index])
                {
                    Roots[item] = Sets[index][0];
                }
            }
        }

        //public bool Find(int item1,int item2)
        //{
            //return Roots[item1] == Roots[item2];
        //}
        public void Unite(int item1,int item2)
        {
            int item1Root = Roots[item1];
            for(int index=0;index<Roots.Count;index++)
            {
                int item = Sets.Keys.ElementAt(index);
                if(Roots[item]==item1Root)
                {
                    Roots[item] = Roots[item2];
                }
            }
        }
        public void Start()
        {
            foreach(int index in Sets.Keys)
            {
                var set = Sets[index];
                for(int i=0;i<set.Count;i++)
                {
                    for(int j=i+1;j<set.Count;j++)
                    {
                        Unite(set[i], set[j]);
                    }
                }
            }
        }
        #endregion
    }
}
