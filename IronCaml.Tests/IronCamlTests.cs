using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml.Tests
{
    public class IronCamlTests
    {
        [Fact]
        public void CodeExample()
        {
            var addTwoNumbers = IronCaml.Execute<Func<long, long, long>> ("let add_two_numbers x y = x + y");
            var result = addTwoNumbers(5, 15);
            Assert.Equal(result, 20);
        }

        [Fact]
        public void TwoLets()
        {
            var addTwoNumbers = IronCaml.Execute<Func<long>>("let x = 3 let y = 7");
            var result = addTwoNumbers();
            Assert.Equal(result, 7);
        }
    }
}
