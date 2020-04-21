using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Levels", order = 1)]
public class Level : ScriptableObject
{
    public hex.HexCell cellPrefab;
    public int width;
    public int height;
    public int food;
    public int water;

    public List<GridGeneration.Building> _buildings;
    public List<GridGeneration.Building> _lakes;
    public List<GridGeneration.Ground> _grounds;
    public List<GridGeneration.Props> _props;
}