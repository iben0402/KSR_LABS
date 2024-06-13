FROM mcr.microsoft.com/dotnet/sdk:8.0
COPY ./ /app
WORKDIR /app
RUN dotnet restore
RUN dotnet publish -c Release -o out
WORKDIR /app/out

EXPOSE 90

CMD ["dotnet", "Library.Web.dll"]