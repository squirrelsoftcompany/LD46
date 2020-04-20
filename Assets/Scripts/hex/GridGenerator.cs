using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SixWay = hex.HexCoordinates.SixWay;

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
        GenerateBuilding(choose(_buildings), new hex.HexCoordinates(3,3), 3, SixWay.ZMinus, new[] { 0, -1, 0});
        GenerateBuilding(choose(_buildings), new hex.HexCoordinates(7,7), 2, SixWay.XMinus, new[] { 0, 0, 0 });
        GenerateBuilding(choose(_buildings), new hex.HexCoordinates(0,11), 1, SixWay.XMinus, new[] { 2, 0, 0 });
    }

    private void GenerateBuilding(GridGeneration.Building building, hex.HexCoordinates startCoords, int radius, SixWay doorWallOrientation, int[] offsetByAxis)
    {
        Debug.Assert(offsetByAxis.Length == 3);
        hex.HexCoordinates current = startCoords.moveAlongAxis((SixWay)(((int)doorWallOrientation + 4) % 6), radius);

        SixWay way = doorWallOrientation;
        current = current.moveAlongAxis(way, 1);

        for (int j = 0; j < 6; j++)
        {
            int rotation = 60 * ((int)way - 1);

            int max = radius + offsetByAxis[j % 3];
            for (int i = 0; i < max - 1; i++)
            {
                if (j == 0 && i == Mathf.CeilToInt((max / 2.0f) - 1))
                    GenerateTopping(choose(building.doors), current, rotation);
                else
                    GenerateTopping(choose(building.walls), current, rotation);
                current = current.moveAlongAxis(way, 1);
            }
            GenerateTopping(choose(building.cornersConvex), current, rotation);

            way = (SixWay)(((int)way + 1) % 6);
            current = current.moveAlongAxis(way, 1);
        }

        FillTopping(building.inside, startCoords);
    }

    private void FillTopping(List<GameObject> toppings, hex.HexCoordinates start)
    {
        GenerateTopping(choose(toppings), start);

        var enumCount = System.Enum.GetNames(typeof(SixWay)).Length;
        for (int w = 0; w < enumCount; w++)
        {
            var neighbor = start.moveAlongAxis((SixWay)w, 1);
            if (_grid.CellAvailable(neighbor))
                FillTopping(toppings, neighbor);
        }
    }

    private void GenerateTopping(GameObject topping, hex.HexCoordinates coordinates, int rotation = 0)
    {
        hex.HexCell cell = _grid[coordinates];
        if (cell != null)
            return;
        Debug.Assert(cell.available());
        _grid[coordinates].topping = GenerateSomething(topping, coordinates, rotation);
    }

    private GameObject GenerateSomething(GameObject something, hex.HexCoordinates coordinates, int rotation = 0)
    {
        var generated = GameObject.Instantiate(something, _grid[coordinates].gameObject.transform);
        generated.transform.localRotation = Quaternion.Euler(0, rotation, 0);
        generated.transform.localPosition = Vector3.zero;
        return generated;
    }

    private T choose<T>(List<T> ts)
    {
        Debug.Assert(ts.Count > 0);
        return ts[Random.Range(0, ts.Count)];
    }
}
