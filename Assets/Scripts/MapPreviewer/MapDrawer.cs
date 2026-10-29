using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public struct MinMaxRoute{
    public Vector2 min;
    public Vector2 max;
}
public class MapDrawer : MonoBehaviour
{
    //Offset for drawing the map elements
    public float offsetX;
    public float offsetY;

    public float maxWidth;
    public float maxHeight;

    public Vector2 minPosition;
    public Vector2 maxPosition;

    public string mapName;
    
    public Image mapPointPrefab;
    public Image routePrefab;
    //Draw mapPoint on map 
    public void DrawMapPoint(Vector2 position)
    {
        if (mapPointPrefab == null)
            return;

      //draw a red circle with a center at the given position
      Image mapPoint = Instantiate(mapPointPrefab, transform);
    // Anchor the image to the canvas's bottom-left corner.
    mapPoint.rectTransform.anchorMin = Vector2.zero;
    mapPoint.rectTransform.anchorMax = Vector2.zero;
      mapPoint.rectTransform.anchoredPosition = new Vector2(
          Mathf.Clamp(position.x+offsetX, 0, maxWidth),
          Mathf.Clamp(position.y+offsetY, 0, maxHeight)
      );
 
      //Set Image height widht as 10 10 
      mapPoint.rectTransform.sizeDelta = new Vector2(1, 1);
    }

    public void DrawJunction(Junction junction)
    {
        Debug.Log("Drawing junction: " + junction.name);
        if (junction == null)
            return;
    
        DrawMapPoint(NormalizePosition(junction.gameObject.transform.position));
    }

    public Vector2 NormalizePosition(Vector3 worldPosition){
        //use world min max position and normalize to map locations 
        float normalizedX = (worldPosition.x - minPosition.x) / (maxPosition.x - minPosition.x) * Mathf.Max(0, maxWidth - 2 * offsetX);
        float normalizedY = (worldPosition.y - minPosition.y) / (maxPosition.y - minPosition.y) * Mathf.Max(0, maxHeight - 2 * offsetY);
        return new Vector2(normalizedX, normalizedY);
    }

    public void ClearCanvas(){
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        minPosition = new Vector2(Mathf.Infinity, Mathf.Infinity);
        maxPosition = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);
        mapName = "";
    }

    public void CalibrateMap(List<Junction> startJunctions){

       
    
       MinMaxRoute result = new MinMaxRoute{
            min = new Vector2(Mathf.Infinity, Mathf.Infinity),
            max = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity)};
       foreach (var startJunction in startJunctions)
       {
           result = RouteMinMaxer(startJunction, result);
       }
       minPosition = result.min;
       maxPosition = result.max;

       Debug.Log("Min Position: " + minPosition + ", Max Position: " + maxPosition);
    }


    public MinMaxRoute RouteMinMaxer(Junction startJunction , MinMaxRoute minMax){
        return RouteMinMaxer(startJunction, minMax, new HashSet<Junction>());
    }

    private MinMaxRoute RouteMinMaxer(Junction startJunction, MinMaxRoute minMax, HashSet<Junction> visited){
        if (startJunction == null || !visited.Add(startJunction))
            return minMax;

        Junction currentJunction = startJunction;
      
        //Temp List of minMax for each route
        MinMaxRoute tempMinMax;
        
        foreach(var nextJunction in currentJunction.GetNextJunctions()){
            tempMinMax = RouteMinMaxer(nextJunction.nextJunction, minMax, visited);
            minMax.min.x = Mathf.Min(minMax.min.x, tempMinMax.min.x);
            minMax.min.y = Mathf.Min(minMax.min.y, tempMinMax.min.y);
            minMax.max.x = Mathf.Max(minMax.max.x, tempMinMax.max.x);
            minMax.max.y = Mathf.Max(minMax.max.y, tempMinMax.max.y);
        }

        //Check with self 
        minMax.min.x = Mathf.Min(minMax.min.x, currentJunction.gameObject.transform.position.x);
        minMax.min.y = Mathf.Min(minMax.min.y, currentJunction.gameObject.transform.position.y);
        minMax.max.x = Mathf.Max(minMax.max.x, currentJunction.gameObject.transform.position.x);
        minMax.max.y = Mathf.Max(minMax.max.y, currentJunction.gameObject.transform.position.y);
        

        return minMax;
    }

    private static readonly Color[] routeColors = {
        Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta
    };

    public void DrawLine(Vector2 from, Vector2 to, Color color)
    {
        if (routePrefab == null)
            return;

        Image line = Instantiate(routePrefab, transform);
        line.color = color;
        RectTransform rt = line.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot = new Vector2(0, 0.5f);

        Vector2 start = new Vector2(
            Mathf.Clamp(from.x + offsetX, 0, maxWidth),
            Mathf.Clamp(from.y + offsetY, 0, maxHeight));
        Vector2 end = new Vector2(
            Mathf.Clamp(to.x + offsetX, 0, maxWidth),
            Mathf.Clamp(to.y + offsetY, 0, maxHeight));
        Vector2 dir = end - start;

        rt.anchoredPosition = start;
        rt.sizeDelta = new Vector2(dir.magnitude, 0.5f);
        // Rotate only in the map plane (Z axis) so lines stay flat on the map.
        rt.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        rt.anchoredPosition3D = new Vector3(start.x, start.y, 0);
    }

    public void DrawMap(List<Junction> startJunctions){
        List<Junction> junctionPoints = new List<Junction>();
        DrawMap(startJunctions, new HashSet<Junction>(), junctionPoints);

        // Draw points after all routes so they appear on top of the lines.
        foreach (Junction junction in junctionPoints)
            DrawJunction(junction);
    }

    private void DrawMap(List<Junction> junctions, HashSet<Junction> visited, List<Junction> junctionPoints){
        foreach (var junction in junctions)
        {
            if (junction == null || !visited.Add(junction))
                continue;

            junctionPoints.Add(junction);

            foreach (var next in junction.GetNextJunctions())
            {
                if (next.nextJunction == null)
                    continue;

                Junction endJunction = next.nextJunction;
                var chain = new HashSet<Junction> { junction };

                // Hide intermediate junctions only along an unbranched route
                // of the same type. Keep endpoints, branches and type changes.
                while (!visited.Contains(endJunction) && chain.Add(endJunction))
                {
                    var outgoing = endJunction.GetNextJunctions()
                        .Where(route => route.nextJunction != null).ToList();
                    if (outgoing.Count != 1 ||
                        !outgoing[0].routeType.Equals(next.routeType) ||
                        chain.Contains(outgoing[0].nextJunction))
                        break;

                    visited.Add(endJunction);
                    endJunction = outgoing[0].nextJunction;
                }

                Color color = routeColors[Mathf.Abs((int)next.routeType) % routeColors.Length];
                DrawLine(
                    NormalizePosition(junction.transform.position),
                    NormalizePosition(endJunction.transform.position),
                    color);

                DrawMap(new List<Junction> { endJunction }, visited, junctionPoints);
            }
        }
    }
    
}