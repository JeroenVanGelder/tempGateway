

FROM microsoft/dotnet

WORKDIR /dotnetapp

# copy project.json and restore as distinct layers
COPY exampleWebAPI.csproj .
RUN dotnet restore

# copy and build everything else
COPY . .
RUN dotnet publish -c Release -o out exampleWebAPI.sln

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "out/exampleWebAPI.dll"]


