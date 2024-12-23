import "./CollapseButton.css";

function CollapseButton(props: { onClick: () => void, collapsed: boolean }) {
    return (
        <div className='sb-button-container'>
            <button className='sb-button' onClick={props.onClick}>
            {props.collapsed ? 'Expand' : 'Collapse'}
            </button>
        </div>
    );
}

export default CollapseButton