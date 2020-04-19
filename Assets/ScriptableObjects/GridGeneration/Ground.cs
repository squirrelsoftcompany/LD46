using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridGeneration
{
    [CreateAssetMenu(fileName = "Ground", menuName = "ScriptableObject/GridGeneration/Ground")]
    public class Ground : ScriptableObject
    {
        public List<GameObject> borders;
        public List<GameObject> inside;
    }
}