using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screwdriver_switch
{
    public class Screwing_dictionary
    {
        public class One_Screwing_parameter
        {
            string product_name;
            string assembly;
            string jig_ID;
            int screwing_time;

            public string Product_name
            {
                get
                {
                    return product_name;
                }
            }

            public string Assembly
            {
                get
                {
                    return assembly;
                }
            }

            public string Jig_ID
            {
                get
                {
                    return jig_ID;
                }
            }

            public int Screwing_time
            {
                get
                {
                    return screwing_time;
                }
            }

            public One_Screwing_parameter(string Product_name, string Assembly, string Jig_ID, int Screwing_time)
            {
                this.product_name = Product_name;
                this.assembly = Assembly;
                this.jig_ID = Jig_ID;
                this.screwing_time = Screwing_time;
            }
        }
    }
}
