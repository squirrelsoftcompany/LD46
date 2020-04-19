using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridGeneration
{
    [CreateAssetMenu(fileName = "Building", menuName = "ScriptableObject/GridGeneration/Building")]
    public class Building : ScriptableObject
    {
        // oriented Z- to Z+
        // inside to X-
        public List<GameObject> doors;
        // oriented Z- to Z+
        // inside to X-
        public List<GameObject> walls;
        // oriented X- to Z+
        // inside to Y+
        public List<GameObject> cornersConvex;
        // oriented X- to Z+
        // inside to Y-
        public List<GameObject> cornersConcave;
        // no specific orientation
        public List<GameObject> inside;
    }
}
