import '../styles/root.css'
import Brand from '../components/Brand';
import Button from '../components/Button';

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
      </div>
    </>
  );
}

export default App