import './Card.css'

function Card(props: { img: string, title: string, description: string, alt: string }) {
    return (
        <div className='card'>
            <img className='card-image' src={props.img} alt={props.alt}></img>
            <h2 className='card-title'>{props.title}</h2>
            <p className='card-text'>{props.description}</p>
        </div>
    );
}

export default Card