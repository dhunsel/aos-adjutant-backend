# syntax=docker/dockerfile:1
FROM node:24-bookworm-slim AS build

WORKDIR /usr/local/app

COPY package.json package-lock.json ./

RUN npm ci

COPY . .

RUN npm run build


FROM caddy:2-alpine

COPY --from=build /usr/local/app/dist /srv

COPY <<EOF /etc/caddy/Caddyfile
:80 {
  root * /srv
  encode zstd gzip
  try_files {path} /index.html
  file_server
}
EOF
