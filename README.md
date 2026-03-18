# Game_Caro_Project_NET 
## Đây là dự án làm game cờ Caro trên Window Form .NET bằng ngôn ngữ C#
**Thời gian làm dự kiến 2 tháng.**
- Yêu cầu:
+ Game trực tuyến 2 người chơi thông qua mạng LAN.
+ Lưu thành tựu người chơi vào Postgres Database.
+ Có thể CRUD (thêm, sửa, xóa, hiển thị) danh sách người chơi.

Ngày bắt đầu: 14/01/2026 <br/>
Ngày kết thúc: 16/03/2026

### Để có thể chạy được file này trước tiên ta cần phải <br/>
### mở file có đuôi .slnx sau đó chạy lệnh dotnet restore để cài các thư viện cần thiết từ NuGet
### sau đó vào file App.config để thay thế connection string mongoDB (1 là chạy localhost, 2 là chạy bằng MongoDB Atlas)
### xong rồi thì enjoy thôi

## Để chơi được 2 người thì cần 2 máy có chứa file dự án này, restore đầy đủ 
## 2 máy phải sử dụng chung 1 mạng internet, rồi 1 máy host, 1 máy join
## máy host sẽ dùng lệnh ipconfig để biết địa chỉ IP của mạng đang dùng là gì
## rồi tạo host, sau đó nhập địa chỉ MAC được sinh ra sau khi nhấn host thay vì địa chỉ IP ở bên máy join
