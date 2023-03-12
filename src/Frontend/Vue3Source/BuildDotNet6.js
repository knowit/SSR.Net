const vuePlugin = require("esbuild-plugin-vue3")
const esbuild = require("esbuild")

esbuild.build({
    entryPoints: ["src/app.tsx"],
    bundle: true,
    outfile: "../../SSR.Net.DotNet6/wwwroot/vue3example.js",
    plugins: [vuePlugin()]
})