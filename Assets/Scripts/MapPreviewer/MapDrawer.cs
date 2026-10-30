using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using System.Reflection;
public struct MinMaxRoute{
    public Vector2 min;
    public Vector2 max;
}

//TODO : add capability for doors , pressureplate , portal
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

    [Serializable]
    public struct TargetSprite
    {
        // Match the targetType name (including enum names).
        public string targetType;
        public Sprite sprite;
    }

    public List<TargetSprite> targetSprites = new List<TargetSprite>();
    public Vector2 targetIconSize = new Vector2(4, 4);

    public void DrawTarget(ITarget target)
    {

        Sprite sprite = null;
        
        targetSprites.ForEach(entry =>
        {
            if (string.Equals(entry.targetType, target.targetType.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                sprite = entry.sprite;
            }
        });
        if (sprite == null)
            return;

        GameObject iconObject = new GameObject("Target: " + target.targetName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        iconObject.transform.SetParent(transform, false);
        Image icon = iconObject.GetComponent<Image>();
        icon.sprite = sprite;
        icon.preserveAspect = true;
        icon.raycastTarget = false;

        Vector2 position = NormalizePosition(target.transform.position);
        RectTransform rect = icon.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.anchoredPosition = new Vector2(
            Mathf.Clamp(position.x + offsetX, 0, maxWidth),
            Mathf.Clamp(position.y+0.5f + offsetY, 0, maxHeight));
        rect.sizeDelta = targetIconSize;
    }

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
      mapPoint.rectTransform.sizeDelta = targetIconSize;

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

    private static float GetRouteTime(object route)
    {
        if (route == null)
            return 0f;

        string[] timeNames = { "routeTime", "travelTime", "time", "duration" };
        Type type = route.GetType();
        foreach (string name in timeNames)
        {
            FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
            PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
            object value = field != null ? field.GetValue(route) : property != null && property.GetIndexParameters().Length == 0 ? property.GetValue(route, null) : null;
            if (value != null)
                return Convert.ToSingle(value);
        }

        return 0f;
    }

    private void DrawRouteTime(Vector2 from, Vector2 to, float routeTime)
    {
        GameObject labelObject = new GameObject("Route Time", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(transform, false);

        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.font = TMP_Settings.defaultFontAsset;
        label.text = routeTime.ToString("0.##");
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.black;
        label.fontSize = 1.5f;
        label.fontStyle = FontStyles.Bold;
        
        label.raycastTarget = false;

        RectTransform rect = label.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.sizeDelta = new Vector2(48, 20);
        rect.anchoredPosition = (from + to) * 0.5f + new Vector2(offsetX, offsetY);
    }

    public void DrawMap(List<Junction> startJunctions , List<ITarget> targets){
        DrawMap(startJunctions, (IEnumerable<ITarget>)targets);
    }

    public void DrawMap(List<Junction> startJunctions, IEnumerable<ITarget> targets){
        List<Junction> junctionPoints = new List<Junction>();
        DrawMap(startJunctions, new HashSet<Junction>(), junctionPoints);

        // Draw points after all routes so they appear on top of the lines.
        foreach (Junction junction in junctionPoints)
            DrawJunction(junction);

        // Draw target icons last so they remain visible above routes and points.
        if (targets != null)
        {
            foreach (ITarget target in targets)
                DrawTarget(target);
        }
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
                float accumulatedRouteTime = GetRouteTime(next);
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

                    accumulatedRouteTime += GetRouteTime(outgoing[0]);
                    visited.Add(endJunction);
                    endJunction = outgoing[0].nextJunction;
                }

                Color color = routeColors[Mathf.Abs((int)next.routeType) % routeColors.Length];
                DrawLine(
                    NormalizePosition(junction.transform.position),
                    NormalizePosition(endJunction.transform.position),
                    color);
                DrawRouteTime(
                    NormalizePosition(junction.transform.position),
                    NormalizePosition(endJunction.transform.position),
                    accumulatedRouteTime);

                DrawMap(new List<Junction> { endJunction }, visited, junctionPoints);
            }
        }
    }
    
}