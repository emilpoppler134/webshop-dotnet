// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const show = element => element.classList.contains("hidden") ? element.classList.remove("hidden") : null;
const hide = element => !element.classList.contains("hidden") ? element.classList.add("hidden") : null;