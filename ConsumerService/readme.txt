https://stackoverflow.com/questions/76074/how-can-i-delete-a-service-in-windows


sc.exe create test_windows_srv binPath= "C:\code\kafka_test\ConsumerService\bin\Debug\net7.0"
sc.exe delete test_windows_srv

sc.exe create test_windows_srv2 binPath= "C:\daemons"


