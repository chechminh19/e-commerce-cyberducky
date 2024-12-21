# Bước 1: Xây dựng từ hình ảnh SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Sao chép csproj gốc và khôi phục các phụ thuộc
COPY E-commerce-cyberDucky/E-commerce-cyberDucky.csproj ./
COPY Application/Application.csproj ./Application/
COPY Domain/Domain.csproj ./Domain/
COPY Infrastructure/Infrastructure.csproj ./Infrastructure/

# Khôi phục các phụ thuộc
RUN dotnet restore

# Sao chép toàn bộ mã nguồn vào trong thư mục làm việc
COPY . .

# Xây dựng ứng dụng
RUN dotnet publish -c Release -o out

# Bước 2: Chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "E-commerce-cyberDucky.dll"]