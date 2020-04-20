using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridGeneration
{
    [CreateAssetMenu(fileName = "Props", menuName = "ScriptableObject/GridGeneration/Props")]
    public class Props : ScriptableObject
    {
        public List<GameObject> props;
    }
}