# Hata ayıklama kapsayıcınızı özelleştirme ve Visual Studio’nun daha hızlı hata ayıklama için görüntülerinizi derlemek üzere bu Dockerfile'ı nasıl kullandığı hakkında bilgi edinmek için https://aka.ms/customizecontainer sayfasına bakın.

# Bu aşama, VS'den hızlı modda çalıştırıldığında kullanılır (Hata ayıklama yapılandırması için varsayılan olarak ayarlıdır)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Bu aşama, hizmet projesini oluşturmak için kullanılır
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/ProductionService/Presentation/ProductionService.WebApi/ProductionService.WebApi.csproj", "src/Services/ProductionService/Presentation/ProductionService.WebApi/"]
COPY ["src/Services/ProductionService/Infrastructure/ProductionService.Persistence/ProductionService.Persistence.csproj", "src/Services/ProductionService/Infrastructure/ProductionService.Persistence/"]
COPY ["src/Services/ProductionService/Core/ProductionService.Domain/ProductionService.Domain.csproj", "src/Services/ProductionService/Core/ProductionService.Domain/"]
COPY ["src/Services/ProductionService/Core/ProductionService.Application/ProductionService.Application.csproj", "src/Services/ProductionService/Core/ProductionService.Application/"]

# 2. ADIM: Restore işlemini kopyaladığın TAM YOL üzerinden yap
RUN dotnet restore "src/Services/ProductionService/Presentation/ProductionService.WebApi/ProductionService.WebApi.csproj"

# 3. ADIM: Tüm dosyaları kopyala
COPY . .
WORKDIR "/src/src/Services/ProductionService/Presentation/ProductionService.WebApi"

RUN dotnet build "./ProductionService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Bu aşama, son aşamaya kopyalanacak hizmet projesini yayımlamak için kullanılır
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductionService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Bu aşama üretimde veya VS'den normal modda çalıştırıldığında kullanılır (Hata Ayıklama yapılandırması kullanılmazken varsayılan olarak ayarlıdır)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductionService.WebApi.dll"]