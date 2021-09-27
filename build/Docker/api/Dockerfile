FROM mcr.microsoft.com/dotnet/sdk:5.0.301-alpine3.13 AS build
ARG Configuration=Release
WORKDIR /app

COPY . ./
RUN dotnet publish code/api/src/PetDoctor.API -c $Configuration -o publish -r alpine-x64

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0.7-alpine3.13
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app/publish
COPY --from=build /app/publish .
ENTRYPOINT ["./PetDoctor.API"]
