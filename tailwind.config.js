const colors = require('tailwindcss/colors')

module.exports = {
    content: [
      '**/*.html',
      '**/*.razor',
      '**/*.cshtml',
      '**/*.cs'
    ],
  darkMode: true, // or 'media' or 'class'
  theme: {
    colors: {
      transparent: 'transparent',
      current: 'currentColor',
      primary : {
        DEFAULT: "var(--md-sys-color-primary)",
        on: "var(--md-sys-color-on-primary)",
        container: "var(--md-sys-color-primary-container)",
        on_container: "var(--md-sys-color-on-primary-container)",
      },
      secondary : {
        DEFAULT: "var(--md-sys-color-secondary)",
        on: "var(--md-sys-color-on-secondary)",
        container: "var(--md-sys-color-secondary-container)",
        on_container: "var(--md-sys-color-on-secondary-container)",
      },
      tertiary : {
        DEFAULT: "var(--md-sys-color-tertiary)",
        on: "var(--md-sys-color-on-tertiary)",
        container: "var(--md-sys-color-tertiary-container)",
        on_container: "var(--md-sys-color-on-tertiary-container)",
      },
      error : {
        DEFAULT: "var(--md-sys-color-error)",
        on: "var(--md-sys-color-on-error)",
        container: "var(--md-sys-color-error-container)",
        on_container: "var(--md-sys-color-on-error-container)",
      },
      background : {
        outline: "var(--md-sys-color-outline)",
        DEFAULT: "var(--md-sys-color-background)",
        on: "var(--md-sys-color-on-background)",
        surface: "var(--md-sys-color-surface)",
        on_surface: "var(--md-sys-color-on-surface)",
      },
    },
    extend: {
    },
  },
  variants: {
    extend: {},
  },
  plugins: [],
}
