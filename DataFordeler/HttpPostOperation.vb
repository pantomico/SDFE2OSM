Public Class HttpPostOperation
    Private formfields As List(Of KeyValuePair(Of String, String))
    Private wc As System.Net.Http.HttpClient
    Private wr As System.Net.Http.HttpRequestMessage
    Public Authentication As System.Net.Http.Headers.AuthenticationHeaderValue
    Public ApiVersion As String = "1.0"
    Private iURL As Uri
    Private iJson As String = String.Empty
    Public LastPostWasSuccess As Boolean
    ''' <summary>
    ''' Prepare HTTP POST operation on API
    ''' </summary>
    ''' <param name="baseUri">API absolute URL</param>
    ''' <param name="relUri">function relative URL</param>
    Public Sub New(baseUri As Uri, relUri As String)
        If String.IsNullOrWhiteSpace(baseUri.AbsolutePath) Then Throw New ArgumentNullException("baseUri")
        If String.IsNullOrWhiteSpace(relUri) Then Throw New ArgumentNullException("relUri")
        Dim uriB As New UriBuilder(baseUri)
        If relUri.StartsWith("/") Then relUri = relUri.Substring(1)
        uriB.Path &= relUri

        Me.iURL = uriB.Uri
        formfields = New List(Of KeyValuePair(Of String, String))
    End Sub
    Public ReadOnly Property URL_Used() As String
        Get
            Return iURL.AbsolutePath
        End Get
    End Property
    Public Sub AddHeader(name As String, value As String)
        Dim df As New KeyValuePair(Of String, String)(name.Trim, value.Trim)
        Me.formfields.Add(df)
    End Sub
    Public Sub SetJsonContent(Json As String)
        iJson = Json
    End Sub
    Public Async Function GetServerResponse() As Task(Of Net.Http.HttpResponseMessage)
        wr = New Net.Http.HttpRequestMessage()
        wr.Method = Net.Http.HttpMethod.Post
        wr.RequestUri = Me.iURL
        Debug.Write("HTTP POST:")
        Debug.WriteLine(Me.iURL)
        If formfields.Count > 0 Then
            wr.Content = New Net.Http.FormUrlEncodedContent(formfields)
        End If

        If Not String.IsNullOrWhiteSpace(iJson) Then
            wr.Content = New Net.Http.StringContent(iJson)
            wr.Content.Headers.ContentType = New Net.Http.Headers.MediaTypeHeaderValue("application/json")
        Else
            Debug.WriteLine("JSON is empty!!!!!")
        End If


        wc = New Net.Http.HttpClient()
        wc.DefaultRequestHeaders.Accept.Clear()
        wc.DefaultRequestHeaders.Accept.Add(New Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json", 1))
        wc.DefaultRequestHeaders.AcceptLanguage.Clear()
        wc.DefaultRequestHeaders.AcceptLanguage.Add(New Net.Http.Headers.StringWithQualityHeaderValue("en", 1))
        If Not (Authentication Is Nothing) Then
            wc.DefaultRequestHeaders.Authorization = Authentication
        End If
        wc.DefaultRequestHeaders.Add("version", ApiVersion)
        For Each a In wc.DefaultRequestHeaders
            Debug.Write(vbTab & "(default)")
            Debug.Write(a.Key)
            Debug.Write(":")
            Debug.WriteLine(a.Value.First.ToString)
        Next
        For Each b In wr.Headers
            Debug.Write(vbTab)
            Debug.Write(b.Key)
            Debug.Write(":")
            Debug.WriteLine(b.Value.First.ToString)
        Next
        Debug.WriteLine(Await wr.Content.ReadAsStringAsync())
        Dim resp As Net.Http.HttpResponseMessage
        Try


            resp = Await wc.PostAsync(wr.RequestUri, wr.Content)
            Debug.WriteLine(resp.ReasonPhrase)

            LastPostWasSuccess = True
        Catch ex As Exception
            resp = Nothing
            LastPostWasSuccess = False
        End Try

        Return resp
    End Function

End Class