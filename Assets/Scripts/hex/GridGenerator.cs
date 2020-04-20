using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SixWay = hex.HexCoordinates.SixWay;

public class GridGenerator
{
    private hex.Grid _grid = null;

    private List<GridGeneration.Building> _buildings;
    private List<GridGeneration.Building> _lakes;
    private List<GridGeneration.Ground> _grounds;
    private List<GridGeneration.Props> _props;

    private int _width;
    private int _height;
    private hex.HexCell _basePrefab;
    private Transform _parent;

    public GridGenerator(hex.Grid grid,
        List<GridGeneration.Building> buildings,
        List<GridGeneration.Building> lakes,
        List<GridGeneration.Ground> grounds,
        List<GridGeneration.Props> props,
        int width, int height, hex.HexCell basePrefab, Transform parent)
    {
        _grid = grid;

        _buildings = buildings;
        _lakes = lakes;
        _grounds = grounds;
        _props = props;

        _width = width;
        _height = height;
        _basePrefab = basePrefab;
        _parent = parent;
    }

    public void Generate()
    {
        //for (int j = _width / 4; j >= 0; j--)
        //{
        //    foreach (var c in hex.ExtensionsHex.GetConvexFormAround(hex.HexCoordinates.FromOffsetCoordinates(_width / 2, _height / 2), (uint)j, new uint[] { 0, 0, 0 }))
        //    {
        //        GenerateGround(_grounds[j%_grounds.Count].inside[0], c);
        //    }
        //}

        foreach (var c in hex.ExtensionsHex.GetDiskAround(hex.HexCoordinates.FromOffsetCoordinates(_width / 2, _height / 2), (uint)_width/2))
        {
            GenerateGround(_grounds[0].inside[0], c);
        }

        //GenerateWaterAreas();
        //GenerateGroundAreas();
        //GenerateBuildings();
        //GenerateProps();
    }

    // Areas

    private void GenerateWaterAreas()
    {
        GenerateWaterArea(choose(_lakes), hex.HexCoordinates.FromOffsetCoordinates(_width/2, _height/2), _width/4);
    }

    private void GenerateGroundAreas()
    {
        int max = 50;
        bool stillNotFull = false;
        while (!stillNotFull && max > 0)
        {
            for (int i = 0; i < 2; i++)
            {
                var c = hex.HexCoordinates.FromOffsetCoordinates(Random.Range(0, _width), Random.Range(0, _height));
                var r = Random.Range(_width / 6, _width / 4);
                GenerateGroundArea(choose(_grounds), c, r);
            }

            bool emptyFound = false;
            for (int i = 0; i < _width && emptyFound; i++)
            {
                for (int j = 0; j < _height && emptyFound; j++)
                {
                    emptyFound = (_grid[hex.HexCoordinates.FromOffsetCoordinates(i, j)] == null);
                }
            }
            stillNotFull = emptyFound;
            max--;
        }
        Debug.Assert(max>0);
    }

    private void GenerateBuildings()
    {
        GenerateBuilding(choose(_buildings), new hex.HexCoordinates(3, 3), 3, SixWay.ZMinus, new[] { 0, -1, 0 });
        GenerateBuilding(choose(_buildings), new hex.HexCoordinates(7, 7), 2, SixWay.XMinus, new[] { 0, 0, 0 });
        GenerateBuilding(choose(_buildings), new hex.HexCoordinates(0, 11), 1, SixWay.XMinus, new[] { 2, 0, 0 });
    }

    public void GenerateProps()
    {

    }

    // Single Area

    private void GenerateWaterArea(GridGeneration.Building lake, hex.HexCoordinates center, int radius)
    {
        createOffset(radius, 3, out var realRadius, out var offset);
        GenerateConvexForm(lake.walls, lake.doors, lake.cornersConvex, center, realRadius, SixWay.XMinus, offset, GenerateWater);

        FillWater(lake.inside, center);
    }

    private void GenerateGroundArea(GridGeneration.Ground ground, hex.HexCoordinates center, int radius)
    {
        createOffset(radius, 3, out var realRadius, out var offset);
        GenerateConvexForm(ground.inside, ground.inside, ground.inside, center, realRadius, SixWay.XMinus, offset, GenerateGround);

        FillGround(ground.inside, center);
    }

    private void GenerateBuilding(GridGeneration.Building building, hex.HexCoordinates startCoords, int radius, SixWay doorWallOrientation, int[] offsetByAxis)
    {
        GenerateConvexForm(building.walls, building.doors, building.cornersConvex, startCoords, radius, doorWallOrientation, offsetByAxis, GenerateTopping);

        FillTopping(building.inside, startCoords);
    }

    // Generic

    private void GenerateConvexForm(List<GameObject> borders, List<GameObject> doors, List<GameObject> corners, hex.HexCoordinates startCoords, int radius, SixWay doorWallOrientation, int[] offsetByAxis, GeneratingFunction func)
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
                    func(choose(doors), current, rotation);
                else
                    func(choose(borders), current, rotation);
                current = current.moveAlongAxis(way, 1);
            }
            func(choose(corners), current, rotation);

            way = (SixWay)(((int)way + 1) % 6);
            current = current.moveAlongAxis(way, 1);
        }
    }

    // Area filler

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

    private void FillGround(List<GameObject> ground, hex.HexCoordinates start)
    {
        GenerateGround(choose(ground), start);

        var enumCount = System.Enum.GetNames(typeof(SixWay)).Length;
        for (int w = 0; w < enumCount; w++)
        {
            var neighbor = start.moveAlongAxis((SixWay)w, 1);
            if (_grid[neighbor] == null)
                FillGround(ground, neighbor);
        }
    }

    private void FillWater(List<GameObject> water, hex.HexCoordinates start)
    {
        GenerateWater(choose(water), start);

        var enumCount = System.Enum.GetNames(typeof(SixWay)).Length;
        for (int w = 0; w < enumCount; w++)
        {
            var neighbor = start.moveAlongAxis((SixWay)w, 1);
            if (_grid[neighbor] == null)
                FillWater(water, neighbor);
        }
    }

    // Cell Generator

    public delegate void GeneratingFunction(GameObject topping, hex.HexCoordinates coordinates, int rotation);

    private void GenerateTopping(GameObject topping, hex.HexCoordinates coordinates, int rotation = 0)
    {
        hex.HexCell cell = _grid[coordinates];
        if (cell == null)
            return;
        Debug.Assert(cell.available());
        _grid[coordinates].topping = GenerateSomething(topping, cell.transform, rotation);
    }

    private void GenerateGround(GameObject ground, hex.HexCoordinates coordinates, int _rotation = 0)
    {
        if (_grid[coordinates] != null)
            return;

        hex.HexCell cell = InitCell(coordinates);
        cell.ground = GenerateSomething(ground, cell.transform, 0);
        cell.type = hex.GroundType.GROUND;
        cell.initMesh();
    }

    private void GenerateWater(GameObject water, hex.HexCoordinates coordinates, int rotation = 0)
    {
        Debug.Assert(_grid[coordinates] == null);
        hex.HexCell cell = InitCell(coordinates);
        cell.ground = GenerateSomething(water, cell.transform, rotation);
        cell.topping = new GameObject(); // fill with an empty go
        cell.topping.transform.parent = cell.transform;
        cell.type = hex.GroundType.WATER;
        cell.initMesh();
    }

    private hex.HexCell InitCell(hex.HexCoordinates c)
    {
        var cell = GameObject.Instantiate(_basePrefab, _parent, false);
        Debug.Assert(cell);

        cell.transform.localPosition = c.ToPosition();
        cell.coordinates = c;
        _grid[cell.coordinates] = cell;
        return cell;
    }

    private GameObject GenerateSomething(GameObject something, Transform parent, int rotation = 0)
    {
        var generated = GameObject.Instantiate(something, parent);
        generated.transform.localRotation = Quaternion.Euler(0, rotation, 0);
        generated.transform.localPosition = Vector3.zero;
        return generated;
    }

    // other

    private T choose<T>(List<T> ts)
    {
        Debug.Assert(ts.Count > 0);
        return ts[Random.Range(0, ts.Count)];
    }

    private void createOffset(int basicRadius, int maxOffset, out int realRadius, out int[] offset)
    {
        realRadius = basicRadius - Random.Range(1, maxOffset);
        offset = new[] { 0, 0, 0 };
        offset[0] = Random.Range(0, basicRadius - realRadius);
        offset[1] = Random.Range(0, basicRadius - realRadius);
        if ((offset[0] == 0 || offset[1] == 0))
            offset[2] = Random.Range(0, basicRadius - realRadius);
    }
}
