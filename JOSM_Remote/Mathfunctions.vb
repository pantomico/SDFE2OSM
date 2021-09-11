Public Class Mathfunctions
    Public Shared Function IsNodeInWay(node As OsmSharp.Node, way As OsmSharp.Complete.CompleteWay) As Boolean
        'latitude= x, longitude = y
        Dim pNode As New Drawing.PointF(node.Latitude, node.Longitude)
        Dim l As New Collections.Generic.List(Of Drawing.PointF)
        For Each node In way.Nodes
            l.Add(New Drawing.PointF(node.Latitude, node.Longitude))
        Next
        Return PointInPolygon(pNode.X, pNode.Y, l.ToArray)
    End Function
    ''' <summary>
    ''' Check if node is in way
    ''' </summary>
    ''' <param name="node">latitude= x, longitude = y</param>
    ''' <param name="way"></param>
    ''' <returns></returns>
    Public Shared Function IsNodeInWay(node As PointF, way As OsmSharp.Complete.CompleteWay) As Boolean
        'latitude= x, longitude = y
        Dim l As New Collections.Generic.List(Of Drawing.PointF)
        For Each pf In way.Nodes
            l.Add(New Drawing.PointF(pf.Latitude, pf.Longitude))
        Next
        Return PointInPolygon(node.X, node.Y, l.ToArray)
    End Function


    ''' <summary>
    ''' Return True if the point is in the polygon.
    ''' </summary>
    ''' <param name="X"></param>
    ''' <param name="Y"></param>
    ''' <returns></returns>
    Private Shared Function PointInPolygon(ByVal X As Single, ByVal Y As Single, Points() As Drawing.PointF) As Boolean
        Dim buffX As Single = X
        Dim buffY As Single = Y
        Dim buffP() As PointF = Points
        Dim max_point As Integer = Points.Length - 1
        Dim total_angle As Single = GetAngle(Points(max_point).X, Points(max_point).Y, X, Y, Points(0).X, Points(0).Y)
        Dim res As Boolean
        For i As Integer = 0 To max_point - 1
            total_angle += GetAngle(Points(i).X, Points(i).Y, X, Y, Points(i + 1).X, Points(i + 1).Y)
        Next
        res = (Math.Abs(total_angle) > 1)
        'If Not res Then
        '    Debug.WriteLine($"{buffX}/{buffY} is not in poly with {buffP(0).X}/{buffP(0).Y} and {buffP(1).X}/{buffP(1).Y}")
        'End If
        Return res
    End Function
    ''' <summary>
    ''' Return the angle ABC.
    '''Return a value between PI And -PI.   
    '''Note that the value Is the opposite of what you might
    '''expect because Y coordinates increase downward.
    ''' </summary>
    ''' <param name="Ax"></param>
    ''' <param name="Ay"></param>
    ''' <param name="Bx"></param>
    ''' <param name="By"></param>
    ''' <param name="Cx"></param>
    ''' <param name="Cy"></param>
    ''' <returns></returns>
    Private Shared Function GetAngle(ByVal Ax As Single, ByVal Ay As Single, ByVal Bx As Single, ByVal By As Single, ByVal Cx As Single, ByVal Cy As Single) As Single
        Dim dot_product As Single = DotProduct(Ax, Ay, Bx, By, Cx, Cy)
        Dim cross_product As Single = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy)
        Return CSng(Math.Atan2(cross_product, dot_product))
    End Function

    ''' <summary>
    ''' Return the dot product AB · BC.
    ''' Note that AB · BC = |AB| * |BC| * Cos(theta).
    ''' </summary>
    ''' <param name="Ax"></param>
    ''' <param name="Ay"></param>
    ''' <param name="Bx"></param>
    ''' <param name="By"></param>
    ''' <param name="Cx"></param>
    ''' <param name="Cy"></param>
    ''' <returns></returns>
    Private Shared Function DotProduct(ByVal Ax As Single, ByVal Ay As Single, ByVal Bx As Single, ByVal By As Single, ByVal Cx As Single, ByVal Cy As Single) As Single
        Dim BAx As Single = Ax - Bx
        Dim BAy As Single = Ay - By
        Dim BCx As Single = Cx - Bx
        Dim BCy As Single = Cy - By
        Return (BAx * BCx + BAy * BCy)
    End Function
    ''' <summary>
    ''' Return the cross product AB x BC.
    ''' The cross product Is a vector perpendicular to AB And BC having length |AB| * |BC| * Sin(theta) And with direction given by the right-hand rule.
    ''' For two vectors in the X-Y plane, the result Is a vector with X And Y components 0 so the Z component' gives the vector's length and direction.
    ''' </summary>
    ''' <param name="Ax"></param>
    ''' <param name="Ay"></param>
    ''' <param name="Bx"></param>
    ''' <param name="By"></param>
    ''' <param name="Cx"></param>
    ''' <param name="Cy"></param>
    ''' <returns></returns>
    Private Shared Function CrossProductLength(ByVal Ax As Single, ByVal Ay As Single, ByVal Bx As Single, ByVal By As Single, ByVal Cx As Single, ByVal Cy As Single) As Single
        Dim BAx As Single = Ax - Bx
        Dim BAy As Single = Ay - By
        Dim BCx As Single = Cx - Bx
        Dim BCy As Single = Cy - By
        Return (BAx * BCy - BAy * BCx)
    End Function

    ''' <summary>
    ''' Simple distance guesser, not very accurate
    ''' </summary>
    ''' <param name="point"></param>
    ''' <param name="way"></param>
    ''' <returns></returns>
    Public Shared Function DistanceToPolygon(point As OsmSharp.Node, way As OsmSharp.Complete.CompleteWay)
        Dim shortest As Double = Double.MaxValue
        Dim dist As Double
        Dim n As OsmSharp.Node
        For Each n In way.Nodes
            dist = Math.Sqrt((point.Latitude - n.Latitude) ^ 2 + (point.Longitude - n.Longitude) ^ 2)
            If dist < shortest Then shortest = dist
        Next
        Return dist
    End Function

End Class
