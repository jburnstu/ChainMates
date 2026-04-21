
export default { SegmentDisplay, RecentSegmentDisplay };
export function SegmentDisplay(props) {

    console.log(props.id)

    let readOnly = true;
    let onChange = null;
    let value = props.fixedContent;

    if (props.isFinalSegment) {
        readOnly = false;
        onChange = props.onChange;
        value = props.currentContent;
    }

    const onClick = () => {
        props.changeSelection(props.id)
    }


    return (
        <textarea className={`segmentDisplay ${readOnly ? undefined : 'currentSegmentDisplay'}`} readOnly={readOnly} value={value}
            onChange={onChange} onClick={onClick} ></ textarea>)

}


export function RecentSegmentDisplay(props) {

    let finalSegment = props.segmentTraceInfo.segmentHistoryList.slice(-1)[0]

    return (
        <div className="recentSegmentDisplayContainer">
            <SegmentDisplay
                id={finalSegment.id}
                isFinalSegment={false}
                fixedContent={finalSegment.content}
                currentContent={null}
                changeSelection={null}
                onChange={null} />
        </div>
    )
}