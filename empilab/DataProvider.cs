using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace empilab
{
    public class DataProvider
    {
        public IEnumerable<Model> Parse(IEnumerable<string> data)
        {
            var list = new List<Model>();
            foreach (var item in data)
            {
                var a = item.Split(new string[] { "     " }, StringSplitOptions.None);
                list.Add(new Model(Convert.ToDouble(a[0].Replace('.', ',')), Convert.ToDouble(a[1].Replace('.', ','))));
            }
            return list;
        }
    }
}
