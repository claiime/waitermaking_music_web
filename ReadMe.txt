____________________________CHẠY CODE__________________
Cach1:Restore database từ file "MusicWebWaitermaking.bak"
Vào file "web.config" thay đổi thông tin kết nối csdl ...vd..server="Servername sqlserver", uid-login user sa,pwd-pass user sa login
vd:
 <connectionStrings>
    <add name="WebDbContextConnectionString" connectionString="server=CLARET\SQLEXPRESS;database=MusicWebWaitermaking;uid=sa;pwd=sa" providerName="System.Data.SqlClient" />
  </connectionStrings>
Cách 2: Thay đổi <connectionString> trong Web.config (bảng phân quyền cần cập nhật lại)

Sau khi kết nối sqlserver, database tự sinh có user mặc định :admin -tly:123456 ;user thường -user01:123456
