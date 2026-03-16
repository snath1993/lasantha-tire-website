/**
 * Type declarations for html2pdf.js
 * @see https://github.com/eKoopmans/html2pdf.js
 */
declare module 'html2pdf.js' {
  interface Html2PdfOptions {
    margin?: number | number[];
    filename?: string;
    image?: { type?: string; quality?: number };
    html2canvas?: Record<string, any>;
    jsPDF?: Record<string, any>;
    enableLinks?: boolean;
    pagebreak?: { mode?: string | string[] };
    [key: string]: any;
  }

  interface Html2PdfInstance {
    set(options: Html2PdfOptions): Html2PdfInstance;
    from(element: HTMLElement | string): Html2PdfInstance;
    outputPdf(type: 'blob'): Promise<Blob>;
    outputPdf(type: 'datauristring'): Promise<string>;
    outputPdf(type: 'arraybuffer'): Promise<ArrayBuffer>;
    save(): Promise<void>;
    then(callback: (pdf: any) => void): Html2PdfInstance;
    toPdf(): Html2PdfInstance;
    toCanvas(): Html2PdfInstance;
    toImg(): Html2PdfInstance;
    toContainer(): Html2PdfInstance;
  }

  function html2pdf(): Html2PdfInstance;
  export default html2pdf;
}
