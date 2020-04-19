using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator
{
    private hex.Grid _grid = null;
    private List<GridGeneration.Building> _buildings;

    public GridGenerator(hex.Grid grid, List<GridGeneration.Building> buildings)
    {
        _grid = grid;
        _buildings = buildings;
    }

    public void Generate()
    {
        GenerateBuilding(choose(_buildings), new hex.HexCoordinates(3,3));
    }

    public void GenerateBuilding(GridGeneration.Building building, hex.HexCoordinates startCoords)
    {
        GenerateTopping(choose(building.doors), startCoords);
        GenerateTopping(choose(building.walls), startCoords.Xplus());
        //GenerateTopping(choose(building.walls), startCoords.Xminus());
        //GenerateTopping(choose(building.walls), startCoords.Zplus(), 60);
        //GenerateTopping(choose(building.walls), startCoords.Zminus(), 60);
        GenerateTopping(choose(building.walls), startCoords.Yplus(), 120);
        //GenerateTopping(choose(building.walls), startCoords.Yminus(), 120);
    }

    public void GenerateTopping(GameObject topping, hex.HexCoordinates coordinates, int rotation = 0)
    {
        _grid[coordinates].topping = GenerateSomething(topping, coordinates, rotation);
    }

    public GameObject GenerateSomething(GameObject something, hex.HexCoordinates coordinates, int rotation = 0)
    {
        var generated = GameObject.Instantiate(something, _grid[coordinates].gameObject.transform);
        generated.transform.localRotation = Quaternion.Euler(0, rotation, 0);
        return generated;
    }

    public T choose<T>(List<T> ts)
    {
        Debug.Assert(ts.Count > 0);
        return ts[Random.Range(0, ts.Count)];
    }
}
