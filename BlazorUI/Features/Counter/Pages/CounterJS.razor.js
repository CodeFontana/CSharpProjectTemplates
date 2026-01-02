export default class extends BlazorJSComponents.Component {
    attach() {
        this.currentCount = 0;
    }

    setParameters({ curCount, btnIncrement, btnReset }) {
        this.curCount = curCount;
        this.setEventListener(btnIncrement, 'click', () => {
            this.currentCount += 1;
            this.render();
        });
        this.setEventListener(btnReset, 'click', () => {
            this.currentCount = 0;
            this.render();
        });
        this.render();
    }

    // A helper to apply changes to DOM content.
    // Not called automatically.
    render() {
        this.curCount.innerText = `Current count: ${this.currentCount}`;
    }
}