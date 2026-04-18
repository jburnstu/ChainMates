
export default { Awards, CircleNotifications };

export function Awards(props) {
    //Leaving this for now!
    return (
        <div className="awardsContainer">
            <header>Awards (coming soon!)
            </header>
        </div>
    );
}


export function CircleNotifications(props) {

    let circleNotificationDTOList = [];

    //useEffect(() => {
    //    circleNotificationDTOList = await contactAPI("notifications/","get",true)
    //})

    return (
        <div className="circleNotificationListContainer">
            <header>Circle Notifications</header>
            <div>
                {circleNotificationDTOList.map((circleNotificationDTO) =>
                    <CircleNotificationPanel key={circleNotificationDTO.id}
                        circleNotificationDTO={circleNotificationDTO} />)}
            </div>
        </div>
    );
}

function CircleNotificationPanel(props) {

    let circleNotificationDTO = props.circleNotificationDTO;

    return (
        <div className="circleNotificationContainer">
            <header>{circleNotificationDTO.circleName}</header>
            <textarea readonly>{circleNotificationDTO.displayName + " added a segment to the story " + circleNotificationDTO.title + " ."}</textarea>
        </div>
    )

}
