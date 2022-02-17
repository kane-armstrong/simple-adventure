FROM mcr.microsoft.com/dotnet/sdk:6.0.100-alpine3.14 AS build
ARG Configuration=Release
WORKDIR /app

COPY code/api/ ./
RUN dotnet publish src/PetDoctor.API -c $Configuration -o publish -r alpine-x64

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0.0-alpine3.14
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app/publish
COPY --from=build /app/publish .
ENTRYPOINT ["./PetDoctor.API"]
