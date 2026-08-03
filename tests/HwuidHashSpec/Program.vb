Imports System.Reflection
Imports System.Text.RegularExpressions

Module Program
    Sub Main()
        Dim asm = Assembly.Load("LicenseChain.VB.SDK")
        Dim clientType = asm.GetTypes().FirstOrDefault(Function(t) t.Name = "LicenseChainClient")
        If clientType Is Nothing Then
            Throw New Exception("LicenseChainClient type not found")
        End If

        Dim ctor = clientType.GetConstructor(New Type() {GetType(String)})
        If ctor Is Nothing Then
            ctor = clientType.GetConstructor(New Type() {GetType(String), GetType(String)})
        End If
        If ctor Is Nothing Then Throw New Exception("LicenseChainClient constructor not found")

        Dim client As Object
        If ctor.GetParameters().Length = 1 Then
            client = ctor.Invoke(New Object() {"test-api-key"})
        Else
            client = ctor.Invoke(New Object() {"test-api-key", "https://api.licensechain.app"})
        End If
        Dim method = clientType.GetMethod("GenerateDefaultHwuid", BindingFlags.NonPublic Or BindingFlags.Instance)
        If method Is Nothing Then
            Throw New Exception("GenerateDefaultHwuid method not found")
        End If

        Dim h1 = CStr(method.Invoke(client, Nothing))
        Dim h2 = CStr(method.Invoke(client, Nothing))

        If h1 <> h2 Then
            Throw New Exception("default hwuid must be deterministic")
        End If
        If Not Regex.IsMatch(h1, "^[a-f0-9]{64}$") Then
            Throw New Exception("default hwuid must be lowercase sha256 hex")
        End If

        Console.WriteLine("HWUID hash spec: ok")
    End Sub
End Module
