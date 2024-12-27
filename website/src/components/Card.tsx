import "./Card.css"

function Card(props: {
    img: string,
    title: string,
    alt: string,
    link?: string,
    subtitle?: string,
    background?: string,
    children?: (JSX.Element | string)[] | JSX.Element | string 
}) {
    //585858

    const title = props.link
        ? <h2 className="card-title"><a href={props.link} target="_blank">{props.title}</a></h2>
        : <h2 className="card-title">{props.title}</h2>;

    return (
        <div className="card">
            <img className="card-image" src={props.img} alt={props.alt} style={{background:props.background}}></img>
            {title}
            <i className="card-subtitle">{props.subtitle}</i>
            <p className="card-text">{props.children}</p>
        </div>
    );
}

export default Card