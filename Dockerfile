
# build client
FROM node:16-alpine as build-client
WORKDIR /build/

COPY /client/package*.json /build/
RUN npm ci

COPY /client/. /build/
ARG GIT_TAG
ENV GIT_TAG=${GIT_TAG:2.0}
ARG GIT_HASH
ENV GIT_HASH=${GIT_HASH:dev}
# no param automatically selects production
RUN node_modules/.bin/webpack --env git-tag=$GIT_TAG --env git-hash=$GIT_HASH
RUN gzip -k /build/dist/*


# build service
FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-service
WORKDIR /build/

COPY /service/*.sln .
COPY /service/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore

COPY /service/. .
RUN dotnet publish -c release -o /release --no-restore


# combine
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /service/
COPY --from=build-service /release .
COPY --from=build-client /build/dist ./wwwroot

RUN mkdir /service/store/
ENTRYPOINT ["/service/GoodNight.Service.Api"]
