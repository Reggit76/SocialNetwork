import os

def collect_text_files(start_path, output_file):
    with open(output_file, 'w', encoding='utf-8') as out_file:
        for root, _, files in os.walk(start_path):
            for file in files:
                if file.endswith('.cs') or file.endswith('.cshtml') or file.endswith('.css') or file.endswith('.js'):
                    file_path = os.path.join(root, file)
                    with open(file_path, 'r', encoding='utf-8') as f:
                        out_file.write(f"{file_path}\n")
                        out_file.write(f.read())
                        out_file.write("\n\n")

if __name__ == "__main__":
    current_directory = os.getcwd()
    output_file = os.path.join(current_directory, 'output.txt')
    collect_text_files(current_directory, output_file)
