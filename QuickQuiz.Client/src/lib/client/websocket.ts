import { BACKEND_BASE_URL } from './requests'

export class WebSocketManager {
    socket?: WebSocket;
    listeners: ((data: any) => void)[];
    connectionAttempts: number;

    constructor() {
        this.socket = undefined;
        this.listeners = [];
        this.connectionAttempts = 0;
        this.init();
    }

    init() {
        fetch(`${BACKEND_BASE_URL}/game/connection-token`, {
            headers: {
                'Authorization': `Bearer ${window.localStorage.getItem('session')}`
            }
        })
            .then((response) => response.json())
            .then(data => {
                if (data.token) {
                    this.socket = new WebSocket(`${BACKEND_BASE_URL}/ws?token=${data.token}`);
                } else {
                    this.socket = new WebSocket(`${BACKEND_BASE_URL}/ws`);
                }
                this.socket.onopen = () => {

                };
                this.socket.onmessage = (event) => {
                    const data = JSON.parse(event.data);
                    this.connectionAttempts = 0;
                    this.listeners.forEach((listener) => listener(data));
                };
                this.socket.onclose = (event) => {
                    if (event.code == 3401 && ++this.connectionAttempts >= 2) {
                        this.listeners.forEach((listener) => listener({
                            error: 'connection_failed'
                        }));
                        return;
                    }

                    if (event.code == 3402) {
                        this.listeners.forEach((listener) => listener({
                            error: 'another_session'
                        }));
                        return;
                    }

                    setTimeout(() => this.init(), 3000);
                };
            })
            .catch(e => {

            });
    }

    sendMessage(message: string) {
        if (this.socket && this.socket.readyState === WebSocket.OPEN) {
            this.socket.send(message);
        } else {
            console.error("WebSocket is not connected.");
        }
    }

    subscribe(callback: (data: any) => void) {
        this.listeners.push(callback);
        return () => {
            this.listeners = this.listeners.filter((listener) => listener !== callback);
        };
    }

    disconnect() {
        if (this.socket) {
            this.socket.close();
        }
    }
}