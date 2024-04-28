import React from 'react';

interface Props{
    header?: string;
    intro?: string;
    text?: string;
}

const ArticlePage = ({header, intro, text}: Props)=>{
return <div><h1>{header}</h1><p style={{fontSize:'24px'}}>{intro}</p><p>{text}</p></div>
}

export default ArticlePage;