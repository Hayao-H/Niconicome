declare namespace application {
    const output: output;
}

interface output {
    write(message: string): void;
    write(message: unknown): void;
}