import * as React from 'react'
import * as ReactDOM  from 'react-dom'
import * as ReactDOMServer from 'react-dom/server'
import FrontPage from './Components/FrontPage'
import ArticlePage from './Components/ArticlePage'

globalThis.React = React;
globalThis.ReactDOM = ReactDOM;
globalThis.ReactDOMServer = ReactDOMServer;
globalThis.Components = {FrontPage, ArticlePage};