FROM mcr.microsoft.com/dotnet/sdk:5.0.100-preview.7-alpine3.12 AS build
ARG Configuration=Release
WORKDIR /app

COPY . ./
RUN dotnet publish src/PetDoctor.API -c $Configuration -o ../../publish -r alpine-x64

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0.0-preview.7-alpine3.12
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
COPY --from=build /publish ./
ENTRYPOINT ["./PetDoctor.API"]
