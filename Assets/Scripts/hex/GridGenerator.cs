using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Dictionary<hex.HexCoordinates, uint> _lakesPosition;
    private Dictionary<hex.HexCoordinates, uint> _buildingsPosition;

    public GridGenerator(hex.Grid grid,
        List<GridGeneration.Building> buildings,
        List<GridGeneration.Building> lakes,
        List<GridGeneration.Ground> grounds,
        List<GridGeneration.Props> props,
        int width, int height, hex.HexCell basePrefab, Transform parent, int seed)
    {
        Random.InitState(seed);

        _grid = grid;

        _buildings = buildings;
        _lakes = lakes;
        _grounds = grounds;
        _props = props;

        _width = width;
        _height = height;
        _basePrefab = basePrefab;
        _parent = parent;

        _lakesPosition = new Dictionary<hex.HexCoordinates, uint>();
        _buildingsPosition = new Dictionary<hex.HexCoordinates, uint>();
    }

    public void Generate()
    {
        GenerateGroundAreas();
        GenerateWaterAreas();
        GenerateBuildings();
        GenerateProps();
    }

    // Areas

    private void GenerateWaterAreas()
    {
        var lakeCount = Random.Range(1, 4);
        for (int i = 0; i < lakeCount; i++)
        {
            var c = GetRandomCellFromOffset();
            var r = (uint)Random.Range(_width / 6, _width / 4);
            while (! checkList(_lakesPosition, c, r+1))
            {
                c = GetRandomCellFromOffset();
                r = (uint)Random.Range(_width / 6, _width / 4);
            }
            GenerateWaterArea(choose(_lakes), c, r);
            _lakesPosition[c] = r;
        }
    }

    private void GenerateGroundAreas()
    {
        var coordsToFill = new List<hex.HexCoordinates>();
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                coordsToFill.Add(hex.HexCoordinates.FromOffsetCoordinates(i, j));
            }
        }

        int max = 50;
        while (coordsToFill.Count > 0 && max > 0)
        {
            var c = choose(coordsToFill);
            var r = (uint)Random.Range(_width / 6, _width / 4);
            GenerateGroundArea(choose(_grounds), c, r);
            
            coordsToFill.RemoveAll(x => _grid[x] != null);
            max--;
        }
        Debug.Assert(max>0);
    }

    private void GenerateBuildings()
    {
        var buildingCount = (_width + _height) / 2;
        buildingCount = buildingCount / 5;
        Debug.Log(buildingCount);
        for (int i = 0; i < buildingCount; i++)
        {
            var c = GetRandomCellFromOffset();
            var r = (uint)Random.Range(2, 6);
            while (!checkList(_lakesPosition, c, r + 1) || !checkList(_buildingsPosition, c, r + 1))
            {
                c = GetRandomCellFromOffset();
                r = (uint)Random.Range(2, 6);
            }
            GenerateBuilding(choose(_buildings), c, r);
            _buildingsPosition[c] = r;
        }
    }

    public void GenerateProps()
    {
        var propsCount = (_width + _height) / 2;
        propsCount = propsCount / 1;
        for (int i = 0; i < propsCount; i++)
        {
            var c = _grid.GetRandomAvailableCell();
            GenerateTopping(choose(choose(_props).props), c, Random.Range(0, 360));
        }
    }

    // Single Area

    private void GenerateWaterArea(GridGeneration.Building lake, hex.HexCoordinates center, uint radius)
    {
        var orientation = (SixWay)Random.Range(0, 6);
        createOffset(radius, radius-2, out var realRadius, out var offset);
        GenerateConvexForm(lake.walls, lake.doors, lake.cornersConvex, center, realRadius, orientation, offset, GenerateWater);
        GenerateFilledConvexForm(lake.inside, center, realRadius-1, orientation, offset, GenerateWater);
    }

    private void GenerateGroundArea(GridGeneration.Ground ground, hex.HexCoordinates center, uint radius)
    {
        var orientation = (SixWay)Random.Range(0, 6);
        createOffset(radius, radius-2, out var realRadius, out var offset);
        GenerateFilledConvexForm(ground.inside, center, realRadius, orientation, offset, GenerateGround);
    }

    private void GenerateBuilding(GridGeneration.Building building, hex.HexCoordinates center, uint radius)
    {
        var orientation = (SixWay)Random.Range(0, 6);
        uint[] offset = new uint[] { radius-1, 0, 0 };
        GenerateConvexForm(building.walls, building.doors, building.cornersConvex, center, 1, orientation, offset, GenerateTopping);
        GenerateFilledConvexForm(building.inside, center, 0, orientation, offset, GenerateTopping);
    }

    // Generic

    private void GenerateConvexForm(List<GameObject> borders, List<GameObject> doors, List<GameObject> corners, hex.HexCoordinates startCoords, uint radius, SixWay doorWallOrientation, uint[] offsetByAxis, GeneratingFunction func)
    {
        var coords = hex.ExtensionsHex.GetConvexFormAround(startCoords, radius, offsetByAxis, doorWallOrientation);

        int rotation = 60 * ((int)doorWallOrientation - 1);

        int current = 0;
        for (int j = 0; j < 6; j++)
        {
            uint max = radius + offsetByAxis[j % 3];
            for (uint i = 0; i < max-1; i++)
            {
                if (j == 0 && i == Mathf.CeilToInt((max / 2.0f) - 1))
                {
                    func(choose(doors), coords[current], rotation);
                    var inventory = _grid[coords[current]]?.GetComponentInChildren<district.BuildingInventory>();
                    inventory?.initFoodWaterPosition(30, 30);
                }
                else
                    func(choose(borders), coords[current], rotation);

                current++;
            }
            func(choose(corners), coords[current], rotation);
            current++;

            rotation += 60;
        }

        Debug.Assert(current == coords.Count);
    }

    private void GenerateFilledConvexForm(List<GameObject> insides, hex.HexCoordinates startCoords, uint radius, SixWay doorWallOrientation, uint[] offsetByAxis, GeneratingFunction func)
    {
        var coords = hex.ExtensionsHex.GetFilledConvexFormAround(startCoords, radius, offsetByAxis, doorWallOrientation);

        foreach (var coord in coords)
        {
            func(choose(insides), coord, 0);
        }
    }

    // Cell Generator

    public delegate void GeneratingFunction(GameObject topping, hex.HexCoordinates coordinates, int rotation);

    private void GenerateTopping(GameObject topping, hex.HexCoordinates coordinates, int rotation = 0)
    {
        hex.HexCell cell = _grid[coordinates];
        if (cell == null)
            return;
        Debug.Assert(cell.available(), coordinates);
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
        var existingCell = _grid[coordinates];
        if (existingCell != null)
            GameObject.Destroy(existingCell.gameObject);
        
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

    private void createOffset(uint basicRadius, uint maxOffset, out uint realRadius, out uint[] offset)
    {
        var diff = (uint)Mathf.Clamp(Random.Range(0, (int)maxOffset+1), 0, basicRadius);
        realRadius = basicRadius - (uint)diff;
        offset = new uint[] { 0, 0, 0 };
        offset[0] = (uint)Random.Range(0, diff);
        offset[1] = (uint)Random.Range(0, diff);
        if ((offset[0] == 0 || offset[1] == 0))
            offset[2] = (uint)Random.Range(0, diff);
    }

    private bool checkList(Dictionary<hex.HexCoordinates, uint> l, hex.HexCoordinates candidate, uint radius)
    {
        foreach (var elem in l)
        {
            if (candidate.DistanceTo(elem.Key) < radius + elem.Value)
                return false;
        }

        return true;
    }

    private hex.HexCoordinates GetRandomCellFromOffset()
    {
        return hex.HexCoordinates.FromOffsetCoordinates(Random.Range(0, _width), Random.Range(0, _height));
    }
}
