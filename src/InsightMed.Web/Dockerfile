FROM node:20-alpine AS build
WORKDIR /usr/src/app

COPY package*.json ./
RUN npm ci

COPY . .

RUN npm run build -- \
  --configuration production

FROM nginx:1.27-alpine AS final

RUN rm -rf /usr/share/nginx/html/*

COPY --from=build \
  /usr/src/app/dist/insightmed-web/browser \
  /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
