const PROXY_CONFIG = [
  {
    context: [
      "/api",
      "/"
    ],
    target: "http://localhost:5114",
    secure: false,
    changeOrigin: true,
  }
]

module.exports = PROXY_CONFIG;
