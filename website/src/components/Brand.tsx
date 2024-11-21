import Footer from './Footer';
import HeaderImage from '../assets/dissonity_header.png';
import './Brand.css'

function Brand() {
    return (
        <>
            <header className='header'>
                <div className='centered-container'>
                    <img className='header-image' src={HeaderImage} alt='Dissonity logo' width='800' />
                    <h1 className='header-title'>Create online experiences using <span className='unity-text'>Unity</span> and <span className='discord-text'>Discord</span></h1>
                </div>
                <div>
                    <p className='sub'>
                        Dissonity is a Unity SDK that allows you to develop, test and release Discord activities. It handles the communication with Discord for you and provides a testing environment within Unity, allowing you to focus on making your app.
                    </p>
                </div>
            </header>
            <Footer />
        </>
    );
}

export default Brand