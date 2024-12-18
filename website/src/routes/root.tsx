import '../styles/root.css'
import Brand from '../components/Brand';
import Button from '../components/Button';
import Footer from '../components/Footer';

function App () {
  return (
    //goto="docs"
    <>
      <div className='main'>
        <Brand />
        <div className='button-container'>
          <Button text='Docs (soon™)' disabled={true}/>
          <Button text='GitHub' link='https://github.com/Furnyr/Dissonity/tree/dev'/>
        </div>
        <Footer />
      </div>
    </>
  );
}

export default App