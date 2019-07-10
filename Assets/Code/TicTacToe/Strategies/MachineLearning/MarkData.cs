using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace MachineLearning
{
    public class MarkData
    {
        public int pos;
        public int val;
        

        public MarkData()
        {

        }

        public MarkData(int position)
        {
            pos = position;
        }

        public MarkData(int position, int _value)
        {
            val = _value;
        }
    }
}
