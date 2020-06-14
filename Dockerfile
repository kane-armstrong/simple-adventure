FROM mcr.microsoft.com/dotnet/core/sdk:3.1.301-alpine3.12 AS build
ARG Configuration=Release
WORKDIR /app

COPY . ./
RUN dotnet publish src/PetDoctor.API -c $Configuration -o ../../publish -r alpine-x64

FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1.5-alpine3.12
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
COPY --from=build /publish ./
ENTRYPOINT ["./PetDoctor.API"]
