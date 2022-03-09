using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Splines.Examples
{
    [CustomSplineDataHandle(typeof(WidthHandleAttribute))]
    public class CustomWidthHandle : SplineDataHandle<float>
    {
        const float k_HandleSize = 0.1f;
        
        public override void DrawDataPoint(
            int controlID, 
            Vector3 position, 
            Vector3 direction,
            Vector3 upDirection,
            SplineData<float> splineData, 
            int dataPointIndex)
        {
            int id1 = controlID;
            int id2 = GUIUtility.GetControlID(FocusType.Passive);

            if(direction == Vector3.zero)
                return;

            if(Event.current.type == EventType.MouseUp
                && Event.current.button != 0
                && ( GUIUtility.hotControl == id1 || GUIUtility.hotControl == id2 ))
            {
                Event.current.Use();
                return;
            }
                
            int previousHotControl = GUIUtility.hotControl;

            var handleColor = Handles.color;
            if(GUIUtility.hotControl == id1 || GUIUtility.hotControl == id2)
                handleColor = Handles.selectedColor;
            else if(GUIUtility.hotControl == 0 && (HandleUtility.nearestControl==id1 || HandleUtility.nearestControl==id2))
                handleColor = Handles.preselectionColor;

            var dataPoint = splineData[dataPointIndex];
            var normalDirection = math.normalize(math.cross(direction, upDirection));

            var extremity1 = position - dataPoint.Value * (Vector3)normalDirection;
            var extremity2 = position + dataPoint.Value * (Vector3)normalDirection;
            Vector3 val1, val2;
            using(new Handles.DrawingScope(handleColor))
            {
                Handles.DrawLine(extremity1, extremity2);
                val1 = Handles.Slider(id1, extremity1, normalDirection,
                    k_HandleSize * HandleUtility.GetHandleSize(position), Handles.CubeHandleCap, 0);
                val2 = Handles.Slider(id2, extremity2, normalDirection,
                    k_HandleSize * HandleUtility.GetHandleSize(position), Handles.CubeHandleCap, 0);
            }

            //We don't want to rebuild the mesh every frame so we use "NoSort" method to avoid sending the 'afterSplineDataWasModified' event
            if(GUIUtility.hotControl == id1)
            {
                if(math.abs((val1 - extremity1).magnitude) > 0)
                    dataPoint.Value = math.abs((val1 - position).magnitude);
                splineData.SetKeyframeNoSort(dataPointIndex, dataPoint);
            }
            else
            if(GUIUtility.hotControl == id2)
            {
                if(math.abs((val2 - extremity2).magnitude) > 0)
                    dataPoint.Value = math.abs((val2 - position).magnitude);
                splineData.SetKeyframeNoSort(dataPointIndex, dataPoint);
            }
            
            //We only refresh and notify event subscribers at the end of the handle manipulation
            if((previousHotControl == id1 || previousHotControl == id2) && GUIUtility.hotControl == 0)
                splineData.SortIfNecessary();
        }
    }
}
