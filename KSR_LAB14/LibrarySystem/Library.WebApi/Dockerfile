FROM mcr.microsoft.com/dotnet/sdk:8.0
COPY ./ /app
WORKDIR /app
RUN dotnet restore
RUN dotnet publish -c Release -o out
WORKDIR /app/out

EXPOSE 91
#RUN dotnet Library.NotificationService2.dll

CMD ["dotnet", "Library.WebApi.dll"]
